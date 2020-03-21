using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using OSMLSGlobalLibrary;
using OSMLSGlobalLibrary.Map;
using OSMLSGlobalLibrary.Modules;

namespace WolvesAndDeer
{
    public class WolvesAndDeer : OSMLSModule
    {
        private Polygon _wildLand;
        private static List<Deer> _deers;
        private static List<Wolf> _wolfs;
        private Wolf _wolf;
        private Deer _deer;
        Random rnd = new Random();
        
        protected override void Initialize()
        {
            //тут будут жить звери
            var wildLandCoordinates = new Coordinate[]
            {
                new Coordinate(6625000, 8710000),
                new Coordinate(6625000, 9000000),
                new Coordinate(7000000, 9000000),
                new Coordinate(7000000, 8710000),
                new Coordinate(6625000, 8710000),
            };

            _wildLand = new Polygon(new LinearRing(wildLandCoordinates));
            MapObjects.Add(_wildLand);
            
            //Спаун оленей
            _deers = new List<Deer>();
            foreach (var coordinate in GenerateCoordinatesAnimal(30))
            {
                var speedX = rnd.Next(-50, 50);
                var speedY = rnd.Next(-50, 50); 
                _deer = new Deer(coordinate, speedX, speedY);
                _deers.Add(_deer);
                MapObjects.Add(_deer);
            }
            
            //Спаун волков
            _wolfs = new List<Wolf>();
            foreach (var coordinate in GenerateCoordinatesAnimal(15))
            {
                _wolf = new Wolf(coordinate);
                _wolfs.Add(_wolf);
                MapObjects.Add(_wolf);
            }
        }
        public override void Update(long elapsedMilliseconds)
        {
            var deers = MapObjects.GetAll<Deer>();
            foreach (var deer in deers)
            {
                if (deer.Coordinate.X < 6625000 - 50) deer._speedX = rnd.Next(0, 50);
                else if (deer.Coordinate.X > 7000000 - 50) deer._speedX = rnd.Next(-50, 0);
                else if (deer.Coordinate.Y < 8710000 - 50) deer._speedY = rnd.Next(0, 50);
                else if (deer.Coordinate.Y > 9000000 - 50) deer._speedY = rnd.Next(-50, 0);
                deer.MoveUpRight();   
            }
        }

        private List<Coordinate> GenerateCoordinatesAnimal(int number)
        {
            var animalList = new List<Coordinate>();
            for (var i = 0; i < number; i++)
            {
                var coordinate = new Coordinate(rnd.Next(6625000, 7000000), rnd.Next(8710000, 9000000));
                animalList.Add(coordinate);
            }
            return animalList;
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
            public double _speedX { get; set; }
            public double _speedY { get; set; }

            public Deer (Coordinate coordinate, double speedX, double speedY) : base(coordinate)
            {
                _speedX = speedX;
                _speedY = speedY;
            }
            
            public void MoveUpRight()
            {
                X += _speedX;
                Y += _speedY;
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
            public Wolf(Coordinate coordinate) : base(coordinate)
            {
                
            }
        }
    }
}
