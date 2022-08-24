using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;

namespace VNEngine
{
    // Creates a new menu when you right click in the Hierarchy pane.
    // Allows the user to easily create dialogue elements
    public class VNEngineEditor : MonoBehaviour
    {
        [MenuItem("VN Engine/Documentation")]
        private static void OpenDocumentation()
        {
            Application.OpenURL(Application.dataPath + "/VN Engine/README.pdf");
        }


        // Imports a .txt file specified by the user.
        [MenuItem("VN Engine/Import script (from .txt file)")]
        private static void ImportTxtScriptFile()
        {
            string path = EditorUtility.OpenFilePanel("Select a script file to import", "", "txt");

            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            Debug.Log("Reading in .txt script file: " + path);

            // Read the file
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(path);

            // Create a child object to hold all elements created from this file
            Transform import_parent = (new GameObject(Path.GetFileNameWithoutExtension(path))).transform;
            ConversationManager cur_conversation = null;

            // Read it line by line
            while ((line = file.ReadLine()) != null)
            {
                // Continue if it's an empty line
                if (String.IsNullOrEmpty(line))
                    continue;
                string[] split_line = line.Split(new char[] { ':' }, 2);

                // Create a new conversation
                if (line.StartsWith("Conversation", true, System.Globalization.CultureInfo.InvariantCulture))
                {
                    GameObject go = new GameObject(split_line[1] + " Conversation");
                    go.transform.parent = import_parent;
                    ConversationManager new_conv = go.AddComponent<ConversationManager>();
                    if (cur_conversation != null)
                    {
                        cur_conversation.start_conversation_when_done = new_conv;
                    }
                    cur_conversation = new_conv;
                }
                else if (line.StartsWith("EnterActor", true, System.Globalization.CultureInfo.InvariantCulture))
                {
                    GameObject go = new GameObject("Enter " + split_line[1]);
                    go.transform.parent = cur_conversation.transform;
                    EnterActorNode node = go.AddComponent<EnterActorNode>();
                    node.actor_name = split_line[1];
                }
                else if (line.StartsWith("ExitActor", true, System.Globalization.CultureInfo.InvariantCulture))
                {
                    GameObject go = new GameObject("Exit " + split_line[1]);
                    go.transform.parent = cur_conversation.transform;
                    ExitActorNode node = go.AddComponent<ExitActorNode>();
                    node.actor_name = split_line[1];
                }
                else if (line.StartsWith("SetBackground", true, System.Globalization.CultureInfo.InvariantCulture))
                {
                    GameObject go = new GameObject("Set Background " + split_line[1]);
                    go.transform.parent = cur_conversation.transform;
                    SetBackground node = go.AddComponent<SetBackground>();
                    try
                    {
                        node.sprite = Resources.Load<Sprite>(split_line[1]);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Error loading audio clip " + split_line[1] + ". Make sure your named clip matches the resource. Ex: some_folder/cool_music");
                    }
                }
                else if (line.StartsWith("SetMusic", true, System.Globalization.CultureInfo.InvariantCulture))
                {
                    GameObject go = new GameObject("Set Music " + split_line[1]);
                    go.transform.parent = cur_conversation.transform;
                    SetMusicNode node = go.AddComponent<SetMusicNode>();
                    // If possible, load the necessary resource
                    try
                    {
                        node.new_music = Resources.Load<AudioClip>(split_line[1]);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Error loading audio clip " + split_line[1] + ". Make sure your named clip matches the resource. Ex: some_folder/cool_music");
                    }
                }
                // ADD MORE HERE IF YOU WISH TO EXTEND THE IMPORTING FUNCTIONALITY
                //
                //
                //
                //
                //

                // Must be a line of dialogue
                else if (split_line.Length == 2)
                {
                    GameObject go = new GameObject(split_line[0]);
                    go.transform.parent = cur_conversation.transform;
                    DialogueNode node = go.AddComponent<DialogueNode>();
                    node.actor = split_line[0];
                    node.textbox_title = split_line[0];
                    node.text = split_line[1];
                }
            }
            file.Close();
            Debug.Log("Done importing script: " + path);
        }




        [MenuItem("GameObject/VN Engine/Create DialogueCanvas", false, 0)]
        private static void CreateDialogueCanvas(MenuCommand menuCommand)
        {
            GameObject go = PrefabUtility.InstantiatePrefab(Resources.Load("DialogueCanvas", typeof(GameObject))) as GameObject;     // Create new object
            go.name = "DialogueCanvas";
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;
        }


        [MenuItem("GameObject/VN Engine/New Conversation", false, 0)]
        private static void CreateConversation(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Conversation");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<ConversationManager>();
        }


        [MenuItem("GameObject/VN Engine/Add Dialogue", false, 0)]
        private static void AddDialogue(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Dialogue");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<DialogueNode>();
            go.AddComponent<AudioSource>();
            go.GetComponent<AudioSource>().playOnAwake = false;
        }


        [MenuItem("GameObject/VN Engine/Actors/Enter Actor", false, 5)]
        private static void EnterActor(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Enter Actor");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<EnterActorNode>();
        }


        [MenuItem("GameObject/VN Engine/Actors/Change Actor Image", false, 0)]
        private static void ChangeActorImage(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Change Actor Image");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<ChangeActorImageNode>();
        }


        [MenuItem("GameObject/VN Engine/Actors/Exit Actor", false, 0)]
        private static void ExitActor(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Exit Actor");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<ExitActorNode>();
        }


        [MenuItem("GameObject/VN Engine/Actors/Exit All Actors", false, 0)]
        private static void ExitAllActors(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Exit All Actors");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<ExitAllActorsNode>();
        }


        [MenuItem("GameObject/VN Engine/Actors/Play Actor Animation", false, 0)]
        private static void PlayActorAnimationNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Play Actor Animation");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<PlayActorAnimationNode>();
        }





        [MenuItem("GameObject/VN Engine/Change Conversation", false, 0)]
        private static void ChangeConversation(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Change Conversation");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<ChangeConversationNode>();
        }



        [MenuItem("GameObject/VN Engine/Misc/Wait", false, 0)]
        private static void Wait(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Wait");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<WaitNode>();
        }


        [MenuItem("GameObject/VN Engine/Branching/Show Choices", false, 0)]
        private static void ShowChoicesNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Show Choices");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<ChoiceNode>();
        }


        [MenuItem("GameObject/VN Engine/Branching/If Then", false, 0)]
        private static void IfNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("If");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<IfNode>();
        }


        [MenuItem("GameObject/VN Engine/Images/Set Background", false, 0)]
        private static void SetBackground(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Set Background");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<SetBackground>();
        }


        [MenuItem("GameObject/VN Engine/Misc/Item", false, 0)]
        private static void ItemNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Item");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<ItemNode>();
        }



        [MenuItem("GameObject/VN Engine/Misc/Hide or Show UI", false, 0)]
        private static void HideShowUI(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Hide/Show UI");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<HideShowUINode>();
        }



        [MenuItem("GameObject/VN Engine/Images/Fade in from Black", false, 0)]
        private static void FadeInFromBlack(MenuCommand menuCommand)
        {
            GameObject go = Instantiate(Resources.Load("Conversation Pieces/Fade in from Black", typeof(GameObject))) as GameObject;     // Create new object
            go.name = "Fade in from Black";
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;
        }


        [MenuItem("GameObject/VN Engine/Images/Fade out to Black", false, 0)]
        private static void FadeToBlack(MenuCommand menuCommand)
        {
            GameObject go = Instantiate(Resources.Load("Conversation Pieces/Fade out to Black", typeof(GameObject))) as GameObject;     // Create new object
            go.name = "Fade out to Black";
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;
        }


        [MenuItem("GameObject/VN Engine/Actors/Move Actor Inwards", false, 0)]
        private static void MoveActorInwardsNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Move Actor Inwards");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<MoveActorInwardsNode>();
        }


        [MenuItem("GameObject/VN Engine/Actors/Move Actor Outwards", false, 0)]
        private static void MoveActorOutwardsNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Move Actor Outwards");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<MoveActorOutwardsNode>();
        }


        [MenuItem("GameObject/VN Engine/Actors/Bring Actor Forward", false, 0)]
        private static void BringActorForwardNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Bring Actor Forward");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<BringActorForwardNode>();
        }


        [MenuItem("GameObject/VN Engine/Actors/Bring Actor Backward", false, 0)]
        private static void BringActorBackwardNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Bring Actor Backward");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<BringActorBackwardNode>();
        }


        [MenuItem("GameObject/VN Engine/Actors/Actor Change Side", false, 0)]
        private static void ActorChangeSideNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Actor Change Side");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<ChangeSideNode>();
        }


        [MenuItem("GameObject/VN Engine/Actors/Darken Actor", false, 0)]
        private static void DarkenActorNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Darken Actor");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<DarkenActorNode>();
        }


        [MenuItem("GameObject/VN Engine/Actors/Brighten Actor", false, 0)]
        private static void BrightenActorNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Brighten Actor");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<BrightenActorNode>();
        }


        [MenuItem("GameObject/VN Engine/Actors/Flip Actor", false, 0)]
        private static void FlipActorFacingNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Flip Actor");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<FlipActorFacingNode>();
        }


        [MenuItem("GameObject/VN Engine/Sound/Play Sound Effect", false, 0)]
        private static void PlaySoundEffectNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Play Sound Effect");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<PlaySoundEffectNode>();
            go.AddComponent<AudioSource>();
            go.GetComponent<AudioSource>().playOnAwake = false;
        }


        [MenuItem("GameObject/VN Engine/Sound/Play Music", false, 0)]
        private static void PlayMusicNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Play Music");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<SetMusicNode>();
        }


        [MenuItem("GameObject/VN Engine/Sound/Fade out Music", false, 0)]
        private static void FadeOutMusicNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Fade out Music");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<SetMusicNode>();
            go.GetComponent<SetMusicNode>().fadeOutPreviousMusic = true;
            go.GetComponent<SetMusicNode>().fadeOutTime = 5.0f;
        }


        [MenuItem("GameObject/VN Engine/Sound/Set Ambience Sounds", false, 0)]
        private static void SetAmbienceSoundsNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Set Ambience");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<AmbienceNode>();
        }


        [MenuItem("GameObject/VN Engine/Sound/Change Audio Snapshot", false, 0)]
        private static void ChangeAudioSnapshotNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Change Audio Snapshot Node");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<ChangeAudioSnapshotNode>();
        }


        [MenuItem("GameObject/VN Engine/Perform Actions", false, 0)]
        private static void PerformActionsNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Perform Actions");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<PerformActionsNode>();
        }



        [MenuItem("GameObject/VN Engine/Branching/Load Scene", false, 0)]
        private static void LoadSceneNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Load Scene");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<LoadSceneNode>();
        }


        [MenuItem("GameObject/VN Engine/Misc/Clear Text", false, 0)]
        private static void ClearTextNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Clear Text");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<ClearTextNode>();
        }


        [MenuItem("GameObject/VN Engine/Misc/Can Save", false, 0)]
        private static void CanSaveNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Can Save");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<CanSaveNode>();
        }


        [MenuItem("GameObject/VN Engine/Images/Static Image", false, 0)]
        private static void StaticImageNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Static Image");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<StaticImageNode>();
        }


        [MenuItem("GameObject/VN Engine/Misc/Set Autoplay", false, 0)]
        private static void SetAutoplayNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Set Autoplay");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<SetAutoplayNode>();
        }


        [MenuItem("GameObject/VN Engine/Stats/Alter Stats", false, 0)]
        private static void AlterStatsNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Alter Stats");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<AlterStatNode>();
        }


        [MenuItem("GameObject/VN Engine/Stats/Generate Random Number", false, 0)]
        private static void GenerateRandomNumberNode(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Generate Random Number");     // Create new object
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;

            go.AddComponent<GenerateRandomNumberNode>();
        }



        [MenuItem("GameObject/VN Engine/Video/Play Video", false, 0)]
        private static void PlayVideo(MenuCommand menuCommand)
        {
            GameObject go = Instantiate(Resources.Load("Conversation Pieces/Play Video", typeof(GameObject))) as GameObject;     // Create new object
            go.name = "Play Video";
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject); // Parent the new object
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);    // Register the creation in the undo system
            Selection.activeObject = go;
        }

    }
}