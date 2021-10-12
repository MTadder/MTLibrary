using System;

namespace MTLibrary {
    /// <summary>
    /// Acts as a wrapper class for position, velocity & rotation Doubles
    /// </summary>
    class Vector {
        #region Internals
        internal struct Rotator {
            public Double Value;
            public Double Velocity;
            public Rotator(Double value, Double velocity) {
                this.Value = value;
                this.Velocity = velocity;
            }
            public override String ToString() {
                return $"{this.Value}^{this.Velocity}";
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
                this.X = x;
                this.Y = y;
            }
            public override String ToString() {
                return $"[{this.X}, {this.Y}]";
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
                (this.X, this.Y) = (this.X + x, this.Y + y);
            }
            public void Add(Coordinate other) {
                this.Add(other.X, other.Y);
            }
            public void Subtract(Double x, Double y) {
                (this.X, this.Y) = (this.X - x, this.Y - y);
            }
            public void Subtract(Coordinate other) {
                this.Subtract(other.X, other.Y);
            }
            public void Multiply(Double by) {
                (this.X, this.Y) = (this.X * by, this.Y * by);
            }
            public void Multiply(Coordinate other) {
                (this.X, this.Y) = (this.X * other.X, this.Y * other.Y);
            }
            public void Divide(Double by) {
                (this.X, this.Y) = (this.X / by, this.Y / by);
            }
            public void Divide(Coordinate other) {
                (this.X, this.Y) = (this.X / other.X, this.Y / other.Y);
            }
            public void Pow(Double toPower) {
                this.X = Math.Pow(this.X, toPower);
                this.Y = Math.Pow(this.Y, toPower);
            }
            public void Pow(Double xPow, Double yPow) {
                this.X = Math.Pow(this.X, xPow);
                this.Y = Math.Pow(this.Y, yPow);
            }
            public void Clamp(Double minX, Double maxX, Double minY, Double maxY) {
                this.X = Math.Clamp(this.X, minX, maxX);
                this.Y = Math.Clamp(this.Y, minY, maxY);
            }
            public void Clamp(Coordinate min, Coordinate max) {
                this.Clamp(min.X, max.X, min.Y, max.Y);
            }
        }
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
        public void Torque(Double amount) {
            this._rotation.Torque(amount);
        }
        public Double GetLength() {
            return this._position.Distance(this._velocity);
        }
        public override String ToString() {
            return $"[{this._position}]^[{this._velocity}]@{this._rotation}";
        }
        #endregion
    }
}
