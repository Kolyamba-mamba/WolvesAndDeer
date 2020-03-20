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
        private Polygon wildLand;
        private List<Deer> deers;
        private List<Wolf> wolfs;
        private Wolf wolf;
        private Deer deer;
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

            wildLand = new Polygon(new LinearRing(wildLandCoordinates));
            MapObjects.Add(wildLand);
            
            //Спаун оленей
            deers = new List<Deer>();
            foreach (var coordinate in GenerateCoordinatesAnimal(10))
            {
                var speedX = rnd.Next(-150, 150);
                var speedY = rnd.Next(-150, 150); 
                deer = new Deer(coordinate, speedX, speedY);
                deers.Add(deer);
                MapObjects.Add(deer);
            }
            
            //Спаун волков
            wolfs = new List<Wolf>();
            foreach (var coordinate in GenerateCoordinatesAnimal(15))
            {
                wolf = new Wolf(coordinate);
                wolfs.Add(wolf);
                MapObjects.Add(wolf);
            }
        }

        public override void Update(long elapsedMilliseconds)
        {
            var deers = MapObjects.GetAll<Deer>();
            foreach (var deer in deers)
            {
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
            public double _speedX { get; }
            public double _speedY { get; }

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
