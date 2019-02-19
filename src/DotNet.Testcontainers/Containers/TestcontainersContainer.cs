namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Images;

  using static LanguageExt.Prelude;

  public class TestcontainersContainer : IDockerContainer
  {
    private bool disposed = false;

    public TestcontainersContainer(
      IDockerImage image,
      IDictionary<string, string> exposedPorts,
      IDictionary<string, string> portBindings)
    {
      this.Image = image;
      this.ExposedPorts = exposedPorts;
      this.PortBindings = portBindings;
    }

    ~TestcontainersContainer()
    {
      this.Dispose(false);
    }

    public string Id { get; private set; }

    private IDockerImage Image { get; }

    private IDictionary<string, string> ExposedPorts { get; }

    private IDictionary<string, string> PortBindings { get; }

    private HostConfig HostConfig
    {
      get
      {
        var portBindings = this.PortBindings.ToDictionary(binding => $"{binding.Key}/tcp", binding => (IList<PortBinding>)List(new PortBinding { HostPort = binding.Value }).ToList());

        return new HostConfig
        {
          PortBindings = portBindings,
        };
      }
    }

    public void Pull()
    {
      // TODO: Implement a proper strategy to wait until the container is up and running.
      TestcontainersClient.Instance.Pull(this.Image.Image).Wait();
    }

    public void Run()
    {
      this.Id = TestcontainersClient.Instance.Run(this.Image.Image, this.HostConfig);
    }

    public void Start()
    {
      TestcontainersClient.Instance.Start(this.Id).Wait();
    }

    public void Stop()
    {
      TestcontainersClient.Instance.Stop(this.Id).Wait();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!this.disposed)
      {
        this.Stop();

        this.disposed = true;
      }
    }
  }
}