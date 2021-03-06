﻿using System;
using System.Diagnostics;
using TwistedOak.Util.Soul;

namespace TwistedOak.Util {
    /// <summary>
    /// Controls the fate of an exposed lifetime.
    /// The exposed lifetime dies or becomes immortal when the EndLifetime or ImmortalizeLifetime methods are called, respectively.
    /// When a source is garbage collected without killing or immortalizing its exposed lifetime, the exposed lifetime becomes immortal.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public sealed class LifetimeSource {
        private readonly MortalSoul _soul = new MortalSoul();

        ///<summary>The lifetime exposed and managed by the lifetime source.</summary>
        public Lifetime Lifetime { get; }

        ///<summary>Creates a new lifetime source managing a new initially mortal lifetime.</summary>
        public LifetimeSource() {
            this.Lifetime = _soul.AsCollapsingLifetime();
        }

        /// <summary>
        /// Permanently transitions the source's exposed lifetime from mortal to dead.
        /// No effect when the exposed lifetime is already dead.
        /// Invalid operation when the exposed lifetime is immortal.
        /// </summary>
        public void EndLifetime() {
            GC.SuppressFinalize(this);
            _soul.TransitionPermanently(Phase.Dead);
        }
        /// <summary>
        /// Permanently transitions the source's exposed lifetime from mortal to immortal.
        /// No effect when the exposed lifetime is already immortal.
        /// Invalid operation when the exposed lifetime is dead.
        /// </summary>
        public void ImmortalizeLifetime() {
            GC.SuppressFinalize(this);
            _soul.TransitionPermanently(Phase.Immortal);
        }

        ///<summary>Permanently transitions the source's exposed lifetime from mortal to immortal.</summary>
        ~LifetimeSource() {
            _soul.TransitionPermanently(Phase.Immortal);
        }

        ///<summary>Returns a text representation of the lifetime source's current state.</summary>
        public override string ToString() => Lifetime.ToString();
    }
}
