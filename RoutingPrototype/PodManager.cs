using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RoutingPrototype
{
    class PodManager : IUpdateDraw
    {
        List<Pod> mPods;
        List<Pod> mFreePods;

        Texture2D mPodTexture;
        Texture2D mDestinationTexture;

        int initialNumberOfPods = 100;

        int currentId = 1;


        Random rnd = new Random();

        RouteManager mRouteManager;

        public PodManager(Texture2D podText, Texture2D destinationText, RouteManager routeManager, Vector2 startLocation)
        {
            mPodTexture = podText;
            mDestinationTexture = destinationText;

            mPods = new List<Pod>();
            mFreePods = new List<Pod>();

            mRouteManager = routeManager;
            //For testing purposes only---
            //mPods.Add(new Pod(mPodTexture, mDestinationTexture, new Vector2(615, 488), routeManager.CityManager, rnd.Next(), currentId++));
            //mPods.Add(new Pod(mPodTexture, mDestinationTexture, new Vector2(589, 522), routeManager.CityManager, rnd.Next(), currentId++));
            //----------------------------

            //establish some number of pods
            for (int i = 0; i < initialNumberOfPods; i++)
            {
                mPods.Add(new Pod(mPodTexture, mDestinationTexture, startLocation, routeManager.CityManager, rnd.Next(), currentId++));
            }
        }

        void skeinAssignment()
        {
            foreach (Pod pod in mPods)
            {
                if (!pod.onFinalApproach && pod.Velocity.Length() > 0.00001f)
                {
                    foreach (Pod otherPod in mPods)
                    {
                        if (!otherPod.onFinalApproach && otherPod.Velocity.Length() > 0.00001f)
                        {
                            if (otherPod != pod)
                            {
                                //Maybe ddd a check to make sure they're not already assigned to the same skein     
                                //Distance Check
                                if ((otherPod.Position - pod.Position).Length() < 30)
                                {
                                    //Similar Route Check (order of this and distance check could be changed)                  
                                    float angle = angleBetweenVectors(pod.CurrentVector, otherPod.CurrentVector); //possibly check if they are in skein and if so use current skein vector
                                    if ((angle < 0.261799 && angle > 0) || (angle > 6.02139 && angle < 6.28319))
                                    //if ((angle < 1 && angle > 0) || (angle > 5 && angle < 6.28319))
                                    {
                                        //Assign pods to be in a skein
                                        if (!pod.inSkein && !otherPod.inSkein)
                                        {
                                            pod.inSkein = true;
                                            pod.inFormation = false;
                                            otherPod.inSkein = true;
                                            otherPod.inFormation = false;
                                            pod.isLeader = true;
                                            pod.Skein = new Skein(pod, otherPod);
                                            otherPod.Skein = pod.Skein;
                                        }
                                        else if (!pod.inSkein && otherPod.inSkein)
                                        {
                                            otherPod.Skein.Members.Add((pod));
                                            pod.inSkein = true;
                                            pod.Skein = otherPod.Skein;
                                            pod.inFormation = false;
                                        }
                                        else if (pod.inSkein && !otherPod.inSkein)
                                        {
                                            pod.Skein.Members.Add((otherPod));
                                            otherPod.inSkein = true;
                                            otherPod.Skein = pod.Skein;
                                            otherPod.inFormation = false;
                                        }
                                        else //both currently in Skeins, thus they need to merge (toying with a few ideas here, going to think about it for a bit)
                                        {

                                        }

                                        //Assign Skein leader
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            skeinAssignment();
            //choose pod to assign to route (use closest?)
            //When route is completed by pod delete route from route list   
            if (mRouteManager.UnassignedRoutes.Count > 0)
            {
                mRouteManager.UnassignedRoutes.Reverse();
                for (int i = mRouteManager.UnassignedRoutes.Count - 1; i >= 0; i--)
                {
                    if (mFreePods.Count > 0)
                    {
                        float shortestDistance = 99999;
                        Pod bestPod = mFreePods.First(); //default assignment so no errors are raised
                        if (mFreePods.Count > 1)
                        {
                            foreach (Pod freePod in mFreePods)
                            {
                                float distnace = (mRouteManager.UnassignedRoutes[i].PickUp - freePod.Position).Length();
                                if (distnace < shortestDistance)
                                {
                                    shortestDistance = distnace;
                                    bestPod = freePod;
                                }
                            }
                        }
                        //assign pod to route
                        bestPod.assignRoute(mRouteManager.UnassignedRoutes[i]);
                        mFreePods.Remove(bestPod);
                        mRouteManager.UnassignedRoutes[i].assigned();
                        mRouteManager.AssignedRoutes.Add(mRouteManager.UnassignedRoutes[i]);
                        mRouteManager.UnassignedRoutes.RemoveAt(i);
                    }
                }
                mRouteManager.UnassignedRoutes.Reverse();
            }       


            foreach (Pod pod in mPods)
            {
                if (pod.isFree() && pod.RecentlyFreed)
                {
                    mFreePods.Add(pod);
                    pod.RecentlyFreed = false;
                    mRouteManager.AssignedRoutes.Remove(pod.CurrentRoute);
                    pod.clearRoute();
                }
                pod.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Pod pod in mPods)
            {
                pod.Draw(spriteBatch);
            }
        }

        float angleBetweenVectors(Vector2 a, Vector2 b)
        {
            a = a / a.Length();
            b = b / b.Length();
            return (float)Math.Acos(Vector2.Dot(a, b));
        }
    }
}
