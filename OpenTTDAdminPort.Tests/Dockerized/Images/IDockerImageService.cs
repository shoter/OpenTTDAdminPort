using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet.Models;

namespace OpenTTDAdminPort.Tests.Dockerized.Images
{
    public interface IDockerImageService
    {
        /// <param name="imageName">Expects full name of the image. example: testtest/sometestimage</param>
        /// <param name="tagName">tag name which we want to remove</param>
        Task RemoveIfExistDockerImage(string imageName, string tagName, CancellationToken token = default);

        Task<ImagesListResponse> SearchForLocalImage(string imageName, string tagName, CancellationToken token = default);

        Task PullImage(string imageName, string tagName, CancellationToken token = default);
    }
}
