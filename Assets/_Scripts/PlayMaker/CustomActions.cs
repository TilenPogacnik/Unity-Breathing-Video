using System;
using HutongGames.PlayMaker;
using UnityEngine;
using System.Collections;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace BreathingLabs.BreathingVideo //TODO: change namespace?
{

	[ActionCategory("_Video")]
	public class WaitForVideosToLoad : FsmStateAction
	{
		
		public override void OnEnter()
		{
			StartCoroutine (WaitForVideos());
		}

		//Waits for VideoController to finish loading all videos 
		IEnumerator WaitForVideos(){
			while (!NewVideoController.instance.allVideosLoaded) {
				yield return new WaitForEndOfFrame();
			}
			//Finish the state (triggers FINISHED event)
			Finish ();
		}
	}

	[ActionCategory("_Video")]
	public class PlayVideo : FsmStateAction
	{
		[Tooltip("Name of the video file. Must include the file extension too.")]
		public FsmString VideoName;
		[Tooltip("If checked then the video will loop. If not checked then the video will stop when it is finished.")]
		public FsmBool Loop;
		[Tooltip("If checked then the video will rewind to the beginning before playing.")]
		public FsmBool SeekToStart;

		//Resets the variables to default state
		public override void Reset()
		{
			VideoName = null;
			Loop = true;
			SeekToStart = true;
		}

		public override void OnEnter()
		{
			if (VideoName == null) {
				Debug.LogError("VideoName can not be null.");
				return;
			}

			if (NewVideoController.instance.allVideosLoaded) {
				 if (NewVideoController.instance.PlayVideo(VideoName.Value, SeekToStart.Value, Loop.Value)){
					Finish ();
				} else {
					Debug.LogError ("There has been an error while trying to play video " + VideoName.Value + ".");
				}
			}
		}
	}

	[ActionCategory("_Video")]
	public class SeekCurrentVideo : FsmStateAction
	{
		[Tooltip("Time in seconds that you want to seek the current video to.")]
		public FsmFloat SeekTime;

		//Resets the variables to default state
		public override void Reset()
		{
			SeekTime = 0.0f;
		}
		
		public override void OnEnter()
		{
			if (NewVideoController.instance.SeekCurrentVideo (SeekTime.Value)) {
				Finish ();
			} else {
				Debug.LogError ("There has been an error while seeking current video.");
			}
		}	
	}

	[ActionCategory("_Video")]
	public class PlayCurrentVideo : FsmStateAction
	{
		public override void OnEnter()
		{
			if (NewVideoController.instance.PlayCurrentVideo()) {
				Finish ();
			} else {
				Debug.LogError ("There has been an error while trying to play current video.");
			}
		}	
	}

	[ActionCategory("_Video")]
	public class PauseCurrentVideo : FsmStateAction
	{
		[Tooltip("If checked the video canvas will be hidden. If not checked the video canvas will stay visible on screen.")]
		public FsmBool HideVideoCanvas;

		public override void Reset(){
			HideVideoCanvas = false;
		}

		public override void OnEnter()
		{
			if (NewVideoController.instance.PauseCurrentVideo(HideVideoCanvas.Value)) {
				Finish ();
			} else {
				Debug.LogError ("There has been an error while trying to pause current video.");
			}
		}	
	}

	[ActionCategory("_Video")]
	public class StopCurrentVideo : FsmStateAction
	{
		[Tooltip("If checked the video canvas will be hidden. If not checked the video canvas will stay visible on screen.")]
		public FsmBool HideVideoCanvas;

		public override void OnEnter()
		{
			if (NewVideoController.instance.StopCurrentVideo(HideVideoCanvas.Value)) {
				Finish ();
			} else {
				Debug.LogError ("There has been an error while trying to stop current video.");
			}
		}	
	}

	[ActionCategory("_Video")]
	public class ShowImage : FsmStateAction
	{
		
		public override void Reset()
		{

		}
		
		public override void OnEnter()
		{
			
		}	
	}
}
