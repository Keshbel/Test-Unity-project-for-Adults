using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
	public class PlayActorAnimationNode : Node
	{
		        
		public Dialogue_Source actor_name_from = Dialogue_Source.Text_From_Editor;
		public string actor_name;
		private string actual_actor_name;
		public string animation_name;


		public override void Run_Node()
		{
			actual_actor_name = VNSceneManager.GetStringFromSource(actor_name, actor_name_from);
			Actor actor = ActorManager.Get_Actor(actual_actor_name);
			// Can only change the animation of an actor if they're already on the scene
			if (actor != null)
			{
				actor.Play_Animation(animation_name);
			} 

			Finish_Node();
		}



		public override void Button_Pressed()
		{

		}



		public override void Finish_Node()
		{
			base.Finish_Node();
		}
	}
}