using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;
using OSMLSGlobalLibrary.Map;
using OSMLSGlobalLibrary.Modules;

namespace WolvesAndDeer
{
    public class WolvesAndDeer : OSMLSModule
    {
        private readonly int _leftX = 6625000;
        private readonly int _rightX = 7000000;
        private readonly int _downY = 8710000;
        private readonly int _upY = 9000000;
        private readonly int _barier = 50;
        private readonly int _wolfSpeed = 15;
        private readonly int _deerSeed = 10;
        private Polygon _wildLand;
        private Wolf _wolf;
        private Deer _deer;
        Random rnd = new Random();

        protected override void Initialize()
        {

            //тут будут жить звери
            var wildLandCoordinates = new[]
            {
                new Coordinate(_leftX, _downY),
                new Coordinate(_leftX, _upY),
                new Coordinate(_rightX, _upY),
                new Coordinate(_rightX, _downY),
                new Coordinate(_leftX, _downY),
            };

            _wildLand = new Polygon(new LinearRing(wildLandCoordinates));
            MapObjects.Add(_wildLand);

            //Спаун оленей
            foreach (var coordinate in GenerateCoordinatesAnimal(200))
            {
                _deer = new Deer(coordinate, _deerSeed);
                MapObjects.Add(_deer);
            }

            //Спаун волков
            foreach (var coordinate in GenerateCoordinatesAnimal(350))
            {
                _wolf = new Wolf(coordinate, _wolfSpeed);
                MapObjects.Add(_wolf);
            }
        }

        public override void Update(long elapsedMilliseconds)
        {
            var deers = MapObjects.GetAll<Deer>();
            var wolves = MapObjects.GetAll<Wolf>();
            
            foreach (var deer in deers)
            {
                //проверяю чтобы олени не вышли из полигона
                if (deer.Coordinate.X < _leftX - _barier || deer.Coordinate.Y < _downY - _barier) 
                    deer._speed = _deerSeed;
                else if (deer.Coordinate.X > _rightX - _barier || deer.Coordinate.Y > _upY - _barier) 
                    deer._speed = -_deerSeed;
                
                deer.MoveUpRight();
            }

            foreach (var wolf in wolves)
            {
                var nearestDeer = deers.Aggregate((deer1, deer2) =>
                    PointExtension.Distance((Point) wolf, (Point) deer1) < PointExtension.Distance((Point) wolf, (Point) deer2) ? deer1 : deer2);
                wolf.MoveUpRight(new Coordinate(nearestDeer.X, nearestDeer.Y));

                if (!wolf.CanEat(nearestDeer)) continue;
                MapObjects.Remove(nearestDeer);
                deers.Remove(nearestDeer);
                var newDeerCoordinate = GenerateCoordinatesAnimal(1)[0];
                MapObjects.Add(new Deer(newDeerCoordinate, 10));
            }
        }

        private List<Coordinate> GenerateCoordinatesAnimal(int number)
        {
            var animalList = new List<Coordinate>();
            for (var i = 0; i < number; i++)
            {
                var coordinate = new Coordinate(rnd.Next(_leftX, _rightX), rnd.Next(_downY, _upY));
                animalList.Add(coordinate);
            }
            
            return animalList;
        }
    }
    
    public static class PointExtension
    {
        public static double Distance(this Point p1, Point p2) => Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));

        public static void Move(this Point p, Coordinate direction, double speed)
        {
            double MinimumDirection(double s, double d) =>
                Math.Min(speed, Math.Abs(s - d)) * Math.Sign(d - s);
            
            p.X += MinimumDirection(p.X, direction.X);
            p.Y += MinimumDirection(p.Y, direction.Y);
        }
    }

//Стиль для оленя
    [CustomStyle(
    @"new ol.style.Style({
        image: new ol.style.Circle({
            opacity: 1.0,
            scale: 1.0,
            radius: 3,
            fill: new ol.style.Fill({
                color: 'rgba(0, 255, 0, 0.6)'
            }),
            stroke: new ol.style.Stroke({
                color: 'rgba(0, 0, 0, 0.4)',
                width: 1
            }),
        })
    });
    ")]
    public class Deer : Point
    {
        public double _speed { get; set; }

        public Deer (Coordinate coordinate, double speed) : base(coordinate)
        {
            _speed = speed;
        }
        
        public void MoveUpRight()
        {
            X += _speed;
            Y += _speed;
        }
    }

    //Стиль для волка
    [CustomStyle(
        @"new ol.style.Style({
        image: new ol.style.Circle({
            opacity: 1.0,
            scale: 1.0,
            radius: 3,
            fill: new ol.style.Fill({
                color: 'rgba(255, 0, 0, 0.6)'
            }),
            stroke: new ol.style.Stroke({
                color: 'rgba(0, 0, 0, 0.4)',
                width: 1
            }),
        })
    });
    ")]
    public class Wolf : Point
    {
        public double _speed { get; }
        public Wolf (Coordinate coordinate, double speed) : base(coordinate)
        {
            _speed = speed;
        }
        public void MoveUpRight(Coordinate direction)
        {
            this.Move(direction, _speed);
        }

        public bool CanEat(Deer deer)
        {
            return PointExtension.Distance(this, deer) < _speed;
        }
    }
}
