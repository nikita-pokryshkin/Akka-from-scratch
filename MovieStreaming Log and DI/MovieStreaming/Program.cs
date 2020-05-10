﻿using System;
using System.Threading;
using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using MovieStreaming.Actors;
using MovieStreaming.Messages;
using MovieStreaming.Statistics;

namespace MovieStreaming
{
    internal class Program
    {
        private static ActorSystem MovieStreamingActorSystem;

        private static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<SimpleTrendingMovieAnalyzer>().As<ITrendingMovieAnalyzer>();
            builder.RegisterType<TrendingMoviesActor>();

            var container = builder.Build();

            MovieStreamingActorSystem = ActorSystem.Create("MovieStreamingActorSystem");

            IDependencyResolver resolver = new AutoFacDependencyResolver(container, MovieStreamingActorSystem);


            MovieStreamingActorSystem.ActorOf(Props.Create<PlaybackActor>(), "Playback");


            do
            {
                ShortPause();

                Console.WriteLine();
                Console.WriteLine("enter a command and hit enter");
                
                var command = Console.ReadLine();

                if (command.StartsWith("play"))
                {
                    int userId = int.Parse(command.Split(',')[1]);
                    string movieTitle = command.Split(',')[2];

                    var message = new PlayMovieMessage(movieTitle, userId);
                    MovieStreamingActorSystem.ActorSelection("/user/Playback/UserCoordinator").Tell(message);
                }

                if (command.StartsWith("stop"))
                {
                    int userId = int.Parse(command.Split(',')[1]);                    

                    var message = new StopMovieMessage(userId);
                    MovieStreamingActorSystem.ActorSelection("/user/Playback/UserCoordinator").Tell(message);
                }

            } while (true);
        }

        // Perform a short pause for demo purposes to allow console to update nicely
        private static void ShortPause()
        {
            Thread.Sleep(450);
        }
    }
}
