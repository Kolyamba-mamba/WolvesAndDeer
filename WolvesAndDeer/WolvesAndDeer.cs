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
            GenerateAnimal(10);

        }

        public override void Update(long elapsedMilliseconds)
        {
            // throw new System.NotImplementedException();
        }

        private void GenerateAnimal(int number)
        {
            Random rnd = new Random(); //происходит троллинг из-за этой строки
            //for (var i = 0; i < number; i++)
            //{
                //var coordinate = new Coordinate(rnd.Next(6625000, 7000000), rnd.Next(8710000, 9000000));
                MapObjects.Add(new Point(6655000, 8910000));
                //animals.Add(new Point(coordinate));
           // }


        }
    }
}
