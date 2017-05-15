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
        Texture2D mLineTexture;

        int initialNumberOfPods = 25;


        Random rnd = new Random();

        RouteManager mRouteManager;

        public PodManager(Texture2D podText, Texture2D destinationText, RouteManager routeManager)
        {
            mPodTexture = podText;
            mDestinationTexture = destinationText;

            mPods = new List<Pod>();
            mFreePods = new List<Pod>();

            mRouteManager = routeManager;

            //establish some number of pods
            for (int i = 0; i < initialNumberOfPods; i++)
            {
                mPods.Add(new Pod(mPodTexture, mDestinationTexture, new Vector2(727, 728), rnd.Next()));
            }
        }

        public void Update(GameTime gameTime)
        {

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




    }
}
