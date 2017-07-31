using System;
using System.Diagnostics;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;

namespace PurinaQnA.Modules
{
    //public class PostUnhandledExceptionToUserOverrideTask : IPostToBot
    //{
    //    private readonly ResourceManager resources;
    //    private readonly IPostToBot inner;
    //    private readonly IBotToUser botToUser;
    //    private readonly TraceListener trace;

    //    public PostUnhandledExceptionToUserOverrideTask(IPostToBot inner, IBotToUser botToUser, ResourceManager resources, TraceListener trace)
    //    {
    //        SetField.NotNull(out this.inner, nameof(inner), inner);
    //        SetField.NotNull(out this.botToUser, nameof(botToUser), botToUser);
    //        SetField.NotNull(out this.resources, nameof(resources), resources);
    //        SetField.NotNull(out this.trace, nameof(trace), trace);
    //    }

    //    public async Task PostAsync(IActivity activity, CancellationToken token)
    //    {
    //        try
    //        {
    //            await inner.PostAsync(activity, token);
    //        }
    //        catch (Exception)
    //        {
    //            try
    //            {
    //                await botToUser.PostAsync("An Error Has Occurred.....", cancellationToken: token);
    //            }
    //            catch (Exception inner)
    //            {
    //                trace.WriteLine(inner);
    //            }

    //            throw;
    //        }
    //    }
    //}
}
