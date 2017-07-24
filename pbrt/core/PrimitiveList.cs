﻿using pbrt.core.geometry;
using System.Collections.Generic;
using System.Linq;
using System;

namespace pbrt.core
{
    // Basic list of primitives, without acceleration structure
    public class PrimitiveList : Aggregate
    {
        public List<Primitive> Primitives { get; private set; }

        private Bounds3<float> worldBounds;

        public PrimitiveList(IEnumerable<Primitive> primitives)
        {
            Primitives = new List<Primitive>(primitives);

            // Cache the world space bounds
            if (Primitives.Any())
            {
                worldBounds = Primitives.First().WorldBounds();
                foreach (var p in Primitives.Skip(1))
                    worldBounds = Bounds3<float>.Union(worldBounds, p.WorldBounds());
            }
        }

        public override bool Intersect(Ray ray, out SurfaceInteraction inter)
        {
            var hit = false;
            float closestT = float.PositiveInfinity;
            inter = null;

            foreach (var p in Primitives)
                if (p.Intersect(ray, out SurfaceInteraction inter2) && ray.Tmax < closestT)
                {
                    hit = true;
                    closestT = ray.Tmax;
                    inter = inter2;
                }

            return hit;
        }

        public override bool IntersectP(Ray ray)
        {
            foreach (var p in Primitives)
                if (p.IntersectP(ray))
                    return true;

            return false;
        }

        public override Bounds3<float> WorldBounds()
        {
            return worldBounds;
        }

        public override void ComputeScatteringFunctions(SurfaceInteraction inter, bool allowMultipleLobes)
        {
            throw new NotImplementedException();
        }
    }
}