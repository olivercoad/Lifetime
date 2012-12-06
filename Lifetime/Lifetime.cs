﻿using System;
using System.Diagnostics;
using TwistedOak.Util.Soul;

namespace TwistedOak.Util {
    /// <summary>
    /// Runs callbacks when transitioning permanently from mortal to either dead or immortal.
    /// The default lifetime is immortal.
    /// Lifetimes whose source is garbage collected are in limbo, meaning they are neither mortal nor dead nor immortal.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct Lifetime {
        /// <summary>
        /// The default lifetime.
        /// A lifetime that has already permanently transitioned from mortal to immortal.
        /// </summary>
        public static readonly Lifetime Immortal = Phase.Immortal.AsPermanentLifetime();
        /// <summary>
        /// NOT the default lifetime.
        /// A lifetime that has already permanently transitioned from mortal to dead.
        /// </summary>
        public static readonly Lifetime Dead = Phase.Dead.AsPermanentLifetime();

        private readonly ISoul _defSoul;
        internal ISoul Soul { get { return _defSoul ?? Util.Soul.Soul.ImmortalSoul; } }
        internal Lifetime(ISoul soul) {
            this._defSoul = soul;
        }

        /// <summary>Determines if this lifetime has permanently transitioned from mortal to immortal.</summary>
        public bool IsImmortal { get { return Soul.Phase == Phase.Immortal; } }
        /// <summary>Determines if this lifetime has permanently transitioned from mortal to dead.</summary>
        public bool IsDead { get { return Soul.Phase == Phase.Dead; } }

        /// <summary>
        /// Registers an action to perform when this lifetime is either dead or immortal.
        /// If a registration lifetime is given and becomes dead before this lifetime becomes dead or immortal, the registration is cancelled.
        /// </summary>
        public void WhenDeadOrImmortal(Action action, Lifetime registrationLifetime = default(Lifetime)) {
            if (action == null) throw new ArgumentNullException("action");
            var s = Soul;
            s.DependentRegister(
                () => { if (s.Phase == Phase.Dead || s.Phase == Phase.Immortal) action(); },
                registrationLifetime.Soul);
        }

        /// <summary>
        /// Registers an action to perform when this lifetime is dead.
        /// If a registration lifetime is given and becomes dead before this lifetime becomes dead, the registration is cancelled.
        /// </summary>
        public void WhenDead(Action action, Lifetime registrationLifetime = default(Lifetime)) {
            if (action == null) throw new ArgumentNullException("action");
            var s = Soul;
            s.DependentRegister(
                () => { if (s.Phase == Phase.Dead) action(); },
                registrationLifetime.Soul);
        }

        /// <summary>
        /// Registers an action to perform when this lifetime is immortal.
        /// If a registration lifetime is given and becomes dead before this lifetime becomes immortal, the registration is cancelled.
        /// </summary>
        public void WhenImmortal(Action action, Lifetime registrationLifetime = default(Lifetime)) {
            if (action == null) throw new ArgumentNullException("action");
            var s = Soul;
            s.DependentRegister(
                () => { if (s.Phase == Phase.Immortal) action(); },
                registrationLifetime.Soul);
        }

        /// <summary>
        /// Determines if two lifetimes are guaranteed to be in the same phase from now on.
        /// Mortal lifetimes are only congruent if they have the same source.
        /// All immortal lifetimes are congruent.
        /// All dead lifetimes are congruent.
        /// All lifetimes in limbo are congruent.
        /// Two initially non-congruent lifetimes can become congruent by ending up in the same non-mortal state.
        /// </summary>
        public bool IsCongruentTo(Lifetime other) {
            if (Equals(this, other)) return true;
            var consistentPhase = Soul.Phase;
            return consistentPhase != Phase.Mortal && consistentPhase == other.Soul.Phase;
        }

        ///<summary>Determines if the other lifetime has the same source.</summary>
        public bool Equals(Lifetime other) {
            return Equals(Soul, other.Soul);
        }
        ///<summary>Returns the hash code for this lifetime, based on its source.</summary>
        public override int GetHashCode() {
            return Soul.GetHashCode();
        }
        ///<summary>Determines if the other object is a lifetime with the same source.</summary>
        public override bool Equals(object obj) {
            return obj is Lifetime && Equals((Lifetime)obj);
        }
        ///<summary>Returns a text representation of the lifetime's current state.</summary>
        public override string ToString() {
            if (Soul.Phase == Phase.Mortal) return "Alive";
            if (Soul.Phase == Phase.Limbo) return "Alive (Limbo)";
            return Soul.Phase.ToString();
        }
    }
}
