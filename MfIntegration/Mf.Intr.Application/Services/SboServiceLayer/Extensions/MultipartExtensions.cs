using Flurl.Http.Content;
using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Services.SboServiceLayer.Extensions;

internal static class MultipartExtensions
{
    internal static Task<IFlurlResponse> PatchMultipartAsync(this IFlurlRequest request, Action<CapturedMultipartContent> buildContent, HttpCompletionOption httpCompletionOption = default, CancellationToken cancellationToken = default(CancellationToken))
    {
        var cmc = new CapturedMultipartContent(request.Settings);
        buildContent(cmc);
        return request.SendAsync(new HttpMethod("PATCH"), cmc, httpCompletionOption, cancellationToken);
    }
}