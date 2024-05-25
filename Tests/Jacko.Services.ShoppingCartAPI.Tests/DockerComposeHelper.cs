using System;
using System.Diagnostics;

namespace Jacko.Services.ShoppingCartAPI.Tests
{
    public class DockerComposeHelper : IDisposable
    {
        private readonly string _dockerComposeFile;

        public DockerComposeHelper(string dockerComposeFile)
        {
            _dockerComposeFile = dockerComposeFile;
        }

        public void Start()
        {
            //ExecuteDockerComposeCommand("up -d --no-deps --build jacko.services.productapi jacko.services.couponapi jacko.services.authapi");
            ExecuteDockerComposeCommand("up -d jacko.services.productapi jacko.services.couponapi jacko.services.authapi");
        }

        public void Stop()
        {
            ExecuteDockerComposeCommand("down jacko.services.productapi jacko.services.couponapi jacko.services.authapi");
        }

        private void ExecuteDockerComposeCommand(string args)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker-compose",
                    Arguments = $"-f {_dockerComposeFile} {args}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                throw new InvalidOperationException($"Docker Compose command failed: {output}\n{error}");
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}

