﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Viscosidad_Scripts
{
	/** Runs a simulation of the flux problem and */
	public class FluxController : MonoBehaviour 
	{
		/* --- Flux problem parameters --- */
	
		/** Flux density. */
		public double density = 0.00093;
	
		/** Flux Viscosity */
		public double viscosity = 0.12;
	
		/** Ball diameter. */
		public double diameter = 0.01;
		
		/** Ball mass. */
		public double mass = 0.01;
	
		/* --- Numeric resolution parameters --- */
		public double paso = 0.001;
		public int n = 2000;
		
		/* Distance to the surface in meters. */
		public double y0 = 0.1;
		private double time = 0;
		
		/** Ready to show results of the simulation. */
		private bool ready = false;
	
		/** Showing simulation. */
		private bool running = false;
		
		/** Speed at which the simulation will be shown. */
		public double speed = 1;
	
		private FluxProblemSolver problemSolver;
		
		public void Start () 
		{
			Debug.Log ("Preparing flux controller");
			var fluxProblem = new FluxProblem (density, diameter, viscosity, mass);
			problemSolver = new FluxProblemSolver(fluxProblem, n, paso, y0);
			reset();
		}
		
		public void reset () 
		{
			running = false;
			time = 0;
			updatePosition(0);
			ready = true;
		}
		
		public void run() 
		{
			if (ready) {
				running = true;
			} else {
				Debug.LogError ("tried to run flux simulation before solution was ready");		
			}
		}
	
		public bool isReady() 
		{
			return ready;
		}
	
		public void stop()
		{
			running = false;
		}
	
		public void stopRunning() 
		{
			running = false;
			ready = false;
		}
	
		public void Update () 
		{
			if (!running) return;
			time += Time.deltaTime * speed;
			updatePosition(time);
			//Debug.Log ("y(" + time + ") = " + transform.localPosition);
		}

		private void updatePosition(double time)
		{
			setPhysicalPosition(problemSolver.getY(time));
		}
	
		/** Set the transform local position given a Y position in physical units (meters). */
		private void setPhysicalPosition(double y)
		{
			transform.localPosition = new Vector3(0f, (float) physicalToLocalPosition(problemSolver.getY(time)), 0f);
		}
		
		/** Transforms a physical position in meters to one in Unity-space units for the ball's local position. */
		private static double physicalToLocalPosition(double y)
		{
			return (y + 0.8) * 3.21;
		}

		/** Gets Drag acceleration for the controllers current time. */
		public double getDragAccel()
		{
			return problemSolver.getDragAccel(time);
		}

		/** Gets gravity acceleration, which is constant. */
		public double getGravityAccel()
		{
			return problemSolver.getGravityAccel();
		}
		
		/** Gets Upthrust acceleration for the controllers current time. */
		public double getUpthrustAccel()
		{
			return problemSolver.getUpthrustAccel(time);
		}

		public double getVelocity()
		{
			return problemSolver.getVelocity(time);
		}

		public void setSpeed(float speed)
		{
			this.speed = speed;
		}
	}
}

