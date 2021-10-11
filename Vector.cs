using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTLibrary {
    class Vector {
        #region Internals
        internal struct Rotator {
            public Double Value;
            public Double Velocity;
            public Rotator(Double value, Double velocity) {
                this.Value = value;
                this.Velocity = velocity;
            }
            public void Torque(Double amount) {
                this.Velocity += amount;
            }
            public void Step(Double deltaTime) {
                this.Value += this.Velocity * deltaTime;
            }
        }
        internal struct Coordinate { 
            public Double X;
            public Double Y;
            public Coordinate(Double x, Double y) {
                this.X = x; this.Y = y;
            }
            public void Step(Coordinate velocity, Double deltaTime) {
                velocity.Multiply(deltaTime);
                this.Add(velocity);
            }
            public Double Distance(Double toX, Double toY) {
                return Math.Sqrt(Math.Pow(toX - this.X, 2) + Math.Pow(toY - this.Y, 2));
            }
            public Double Distance(Coordinate to) {
                return this.Distance(to.X, to.Y);
            }
            public void Add(Double x, Double y) {
                this.X += x; this.Y += y;
            }
            public void Add(Coordinate other) {
                this.Add(other.X, other.Y);
            }
            public void Subtract(Double x, Double y) {
                this.X -= x; this.Y -= y;
            }
            public void Subtract(Coordinate other) {
                this.Subtract(other.X, other.Y);
            }
            public void Multiply(Double by) {
                this.X *= by; this.Y *= by;
            }
            public void Multiply(Coordinate other) {
                this.X *= other.X; this.Y *= other.Y;
            }
            public void Divide(Double by) {
                this.X /= by; this.Y /= by;
            }
            public void Divide(Coordinate other) {
                this.X /= other.X; this.Y /= other.Y;
            }
            public void Pow(Double toPower) {
                this.X = Math.Pow(this.X, toPower);
                this.Y = Math.Pow(this.Y, toPower);
            }
            public void Clamp(Double minX, Double maxX, Double minY, Double maxY) {
                this.X = Math.Clamp(this.X, minX, maxX);
                this.Y = Math.Clamp(this.Y, minY, maxY);
            }
            public void Clamp(Coordinate min, Coordinate max) {
                this.Clamp(min.X, max.X, min.Y, max.Y);
            }
        };
        internal Coordinate _position;
        internal Rotator _rotation;
        internal Coordinate _velocity;
        internal Vector() {
            this._position = new(0D, 0D);
            this._rotation = new(0D, 0D);
            this._velocity = new(0D, 0D);
        }
        #endregion
        #region Contructors
        public Vector(Double x, Double y) : this() {
            this._position.Add(x, y);
        }
        public Vector(Double x, Double y, Double rotation) : this() {
            this._position.Add(x, y);
            this._rotation.Value = rotation;
        }
        public Vector(Double x, Double y, Double xV, Double yV) : this() {
            this._position.Add(x, y);
            this._velocity.Add(xV, yV);
        }
        public Vector(Double x, Double y, Double xV, Double yV,
            Double rotation, Double radialVelocity) : this() {
            this._position.Add(x, y);
            this._velocity.Add(xV, yV);
            this._rotation.Value = rotation;
            this._rotation.Velocity = radialVelocity;
        }
        #endregion
        #region Properties
        public Coordinate Position {
            get { return this._position; }
            set { this._position = value; }
        }
        public Coordinate Velocity {
            get { return this._velocity; }
            set { this._velocity = value; }
        }
        public Rotator Rotation {
            get { return this._rotation; }
            set { this._rotation = value; }
        }
        #endregion
        #region Methods
        public void Step(Double deltaTime) {
            this._position.Step(this._velocity, deltaTime);
            this._rotation.Step(deltaTime);
        }
        #endregion
    }
}
