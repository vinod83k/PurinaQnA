//using System;
//using System.Diagnostics;
//using System.Resources;
//using Autofac;
//using Microsoft.Bot.Builder.Dialogs;
//using Microsoft.Bot.Builder.Dialogs.Internals;
//using Microsoft.Bot.Builder.History;
//using Microsoft.Bot.Builder.Internals.Fibers;
//using Microsoft.Bot.Connector;
//using Microsoft.Bot.Builder.Scorables.Internals;

//namespace PurinaQnA.Modules
//{
//    public class DefaultExceptionMessageOverrideModule : Module
//    {
//        protected override void Load(ContainerBuilder builder)
//        {

//            builder
//                .Register(c =>
//                {
//                    var cc = c.Resolve<IComponentContext>();
//                    Func<IPostToBot> makeInner = () =>
//                    {
//                        var task = cc.Resolve<DialogTask>();
//                        IDialogStack stack = task;
//                        IPostToBot post = task;
//                        post = new ReactiveDialogTask(post, stack, cc.Resolve<IStore<IFiberLoop<DialogTask>>>(),
//                            cc.Resolve<Func<IDialog<object>>>());
//                        post = new ExceptionTranslationDialogTask(post);
//                        post = new LocalizedDialogTask(post);
//                        post = new ScoringDialogTask<double>(post, stack,
//                            cc.Resolve<TraitsScorable<IActivity, double>>());
//                        return post;
//                    };

//                    IPostToBot outer = new PersistentDialogTask(makeInner, cc.Resolve<IBotData>());
//                    outer = new SerializingDialogTask(outer, cc.Resolve<IAddress>(), c.Resolve<IScope<IAddress>>());

//                    // --- our new class here
//                    outer = new PostUnhandledExceptionToUserOverrideTask(outer, cc.Resolve<IBotToUser>(),

//                        cc.Resolve<ResourceManager>(), cc.Resolve<TraceListener>());
//                    outer = new LogPostToBot(outer, cc.Resolve<IActivityLogger>());
//                    return outer;
//                })
//                .As<IPostToBot>()
//                .InstancePerLifetimeScope();

//        }
//    }
//}