﻿using Akka.Actor;
using MovieStreaming.Common.Exceptions;
using MovieStreaming.Common.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieStreaming.Common.Actors
{
    public class MoviePlayCounterActor : ReceiveActor
    {
        private readonly Dictionary<string, int> _moviePlayCounts;

        public MoviePlayCounterActor()
        {
            ColorConsole.WriteMagenta("MoviePlayCounter constructor exec");
            _moviePlayCounts = new Dictionary<string, int>();

            Receive<IncrementPlayCountMessage>(message => HandleMessage(message));
        }

        private void HandleMessage(IncrementPlayCountMessage message)
        {
            if (_moviePlayCounts.ContainsKey(message.MovieTitle))
            {
                _moviePlayCounts[message.MovieTitle]++;
            }
            else
            {
                _moviePlayCounts.Add(message.MovieTitle, 1);
            }

            //bugs
            if(_moviePlayCounts[message.MovieTitle] > 3)
            {
                throw new SimulatedCorruptStateException();
            }
            if (message.MovieTitle == "the Movie")
            {
                throw new SimulatedTerribleMovieException();
            }

            ColorConsole.WriteMagenta(
                $"MoviePlayActor {message.MovieTitle} has been watched {_moviePlayCounts[message.MovieTitle]} times"
                );
        }

        #region Lifecycle hooks

        protected override void PreStart()
        {
            ColorConsole.WriteMagenta("MoviePlayCounterActor PreStart");
        }

        protected override void PostStop()
        {
            ColorConsole.WriteMagenta("MoviePlayCounterActor PostStop");
        }

        protected override void PreRestart(Exception reason, object message)
        {
            ColorConsole.WriteMagenta($"MoviePlayCounterActor PreRestart because: {reason.Message}");

            base.PreRestart(reason, message);
        }

        protected override void PostRestart(Exception reason)
        {
            ColorConsole.WriteMagenta($"MoviePlayCounterActor PostRestart because: {reason.Message} ");

            base.PostRestart(reason);
        }
        #endregion
    }
}
