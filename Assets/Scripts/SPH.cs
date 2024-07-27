using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

[System.Serializable]
[StructLayout(LayoutKind.Sequential, Size = 44)]
public struct Particle // 44 bytes, has the various properties of the particle
{
    public float pressure;
    public float density;
    public Vector3 currentForce;
    public Vector3 velocity;
    public Vector3 position;
}

public class SPH : MonoBehaviour
{
    [Header("General Settings")]
    public bool showSpheres = true;
    public Vector3Int numToSpawn = new Vector3Int(10, 10, 10); //particles to be spawned in x, y, z
    private int totalParticles
    {
        get
        {
            return numToSpawn.x * numToSpawn.y * numToSpawn.z; //total number of particles to be spawned
        }
    }
    public Vector3 boxSize = new Vector3(5, 7, 5);
    public Vector3 spawnCenter;
    public static float particleRadius = 0.1f;
    [Header("Particle Rendering")]
    public Mesh particleMesh; // tp link the mesh of the particle
    public Material particleMaterial; // to link the material of the particle
    [SerializeField]
    public static float particleRenderSize = 8f;
    [SerializeField]
    public static float spawnJitter = 0.1f;
    [Header("Compute Shader")]
    public ComputeShader shader; // to link the compute shader
    public Particle[] particles; // array of particles
    [Header("Fluid Properties")]
    public float particleMass = 1f;
    public float viscosity = -0.003f;
    public float restDensity = 1f;
    public float gasConstant = 2f;
    public float boundDamping = -0.5f;
    public float timeStep = 0.0008f;
    //private variables
    public ComputeBuffer _particlesBuffer;
    public ComputeBuffer argsBuffer;
    private int integrateKernel;
    private int forcesKernel;
    private int densityPressureKernel;

    private static readonly int SizeProperty = Shader.PropertyToID("_size");
    private static readonly int ParticleBufferProperty = Shader.PropertyToID("_particlesBuffer");
    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        particleMaterial.SetFloat(SizeProperty, particleRenderSize);
        particleMaterial.SetBuffer(ParticleBufferProperty, _particlesBuffer);

        if (showSpheres)
        {
            Graphics.DrawMeshInstancedIndirect(
                particleMesh,
                0,
                particleMaterial,
                new Bounds(Vector3.zero, boxSize),
                argsBuffer,
                castShadows: UnityEngine.Rendering.ShadowCastingMode.Off);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            RunSimulation();
        }
    }
    //FixedUpdate is called at a fixed interval and is independent of frame rate so we can customize the time step for the physics calculations
    private void FixedUpdate()
    {
        shader.SetVector("boxSize", boxSize);
        shader.SetFloat("timeStep", timeStep);
        shader.Dispatch(densityPressureKernel, totalParticles / 100, 1, 1);
        shader.Dispatch(forcesKernel, totalParticles / 100, 1, 1);
        shader.Dispatch(integrateKernel, totalParticles / 100, 1, 1);
        particleMaterial.SetBuffer(ParticleBufferProperty, _particlesBuffer);
        particleMaterial.SetFloat(SizeProperty, particleRenderSize);
    }

    private void Awake()
    {
        RunSimulation();
    }

    public void RunSimulation()
    {
        SpawnParticlesInBox();
        //this args array is used to tell the GPU how many particles to draw
        uint[] args = {
            particleMesh.GetIndexCount(0),  // number of indices of the mesh of the particle
            (uint)totalParticles,
            particleMesh.GetIndexStart(0),  // starting index of the mesh of the particle
            particleMesh.GetBaseVertex(0), // starting vertex of the mesh of the particle, its an offset added to all vertices in the submesh
            0
        };
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);// we have to pass the length of the array and the size of the array to make the buffer
        argsBuffer.SetData(args);

        _particlesBuffer = new ComputeBuffer(totalParticles, 44);
        _particlesBuffer.SetData(particles);

        SetupComputeBuffers();//call the function to setup the compute buffers that links the compute shader with the particle buffer
    }

    private void SpawnParticlesInBox()
    {
        Vector3 spawnPoint = spawnCenter;
        List<Particle> particleList = new();
        for (int x = 0; x < numToSpawn.x; x++)// loop to spawn the particles in x, y, z
        {
            for (int y = 0; y < numToSpawn.y; y++)
            {
                for (int z = 0; z < numToSpawn.z; z++)
                {
                    Vector3 particlePosition = new(
                        spawnPoint.x + x * particleRadius * 2,
                        spawnPoint.y + y * particleRadius * 2,
                        spawnPoint.z + z * particleRadius * 2
                    ); // arranging the particles in a grid like structure
                    particlePosition += particleRadius * spawnJitter * Random.insideUnitSphere; // adding some randomness to the position of the particles
                    Particle p = new Particle
                    {
                        position = particlePosition,
                        velocity = Vector3.zero,
                        density = restDensity,
                        pressure = 0,
                        currentForce = Vector3.zero
                    };
                    particleList.Add(p); // adding the particle to the list
                }
            }
        }
        particles = particleList.ToArray(); // storing the particles in the array for the compute shader
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
        Gizmos.DrawWireSphere(spawnCenter, 0.5f);
    }

    private void SetupComputeBuffers()
    {

        densityPressureKernel = shader.FindKernel("DensityPressure");
        forcesKernel = shader.FindKernel("Forces");
        integrateKernel = shader.FindKernel("Integrate");

        shader.SetBuffer(densityPressureKernel, "_particles", _particlesBuffer);
        shader.SetBuffer(forcesKernel, "_particles", _particlesBuffer);
        shader.SetBuffer(integrateKernel, "_particles", _particlesBuffer);

        // shader parameters
        shader.SetFloat("particleMass", particleMass);
        shader.SetFloat("viscosity", viscosity);
        shader.SetFloat("restDensity", restDensity);
        shader.SetFloat("gasConstant", gasConstant);
        shader.SetFloat("boundDamping", boundDamping);
        shader.SetFloat("radius", particleRadius);
        shader.SetFloat("radius2", Mathf.Pow(particleRadius, 2));
        shader.SetFloat("radius3", Mathf.Pow(particleRadius, 3));
        shader.SetFloat("radius4", Mathf.Pow(particleRadius, 4));
        shader.SetFloat("radius5", Mathf.Pow(particleRadius, 5));
        shader.SetFloat("pi", Mathf.PI);
        shader.SetInt("particleLength", totalParticles);
        shader.SetFloat("timeStep", timeStep);
        shader.SetVector("boxSize", boxSize);
    }
}