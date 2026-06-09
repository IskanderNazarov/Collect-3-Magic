using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _game;
using _game._GameViews;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class PlayModeTestRunner {
    static PlayModeTestRunner() {
        string state = SessionState.GetString("PlayModeTest.State", "Idle");
        if (state == "WaitingForCompile") {
            SessionState.SetString("PlayModeTest.State", "EnteringPlayMode");
            EditorApplication.delayCall += () => EditorApplication.isPlaying = true;
        } else if (state == "EnteringPlayMode" && EditorApplication.isPlaying) {
            SessionState.SetString("PlayModeTest.State", "InPlayMode");
            var go = new GameObject("TestBootstrap");
            go.AddComponent<TestBootstrapComponent>();
        }
    }

    private class TestBootstrapComponent : MonoBehaviour {
        private IEnumerator Start() {
            yield return new WaitForSeconds(2f);
            Debug.Log("[Test] Starting Play Mode Test...");

            var gameplayView = FindFirstObjectByType<GameplayView>();
            if (gameplayView == null) {
                Finish("GameplayView not found");
                yield break;
            }

            // Find a triplet to match
            var activeBubbles = FindObjectsByType<BubbleView>(FindObjectsSortMode.None).Where(b => b.gameObject.activeSelf).ToList();
            var allItems = activeBubbles.SelectMany(b => b.GetComponentsInChildren<ItemView>()).ToList();
            
            // Find target types from active critters
            var containers = FindObjectsByType<ContainerView>(FindObjectsSortMode.None).Where(c => c.gameObject.activeSelf && c.TargetType != ItemType.None).ToList();
            
            if (containers.Count == 0) {
                Finish("No active critters found");
                yield break;
            }

            var targetType = containers[0].TargetType;
            var targetItems = allItems.Where(i => i.Type == targetType).Take(3).ToList();

            if (targetItems.Count < 3) {
                Finish("Not enough items of type " + targetType + " to perform match");
                yield break;
            }

            Debug.Log("[Test] Matching " + targetType);

            foreach (var item in targetItems) {
                // Simulate click via OnMouseDown
                item.SendMessage("OnMouseDown");
                yield return new WaitForSeconds(0.2f);
            }

            // Wait for flight and animation
            yield return new WaitForSeconds(3f);

            // Check if critter respawned or if candidates were found in logs
            Finish("Match completed. Check logs for [GameplayController] output.");
        }

        private void Finish(string result) {
            Debug.Log("[Test] Result: " + result);
            SessionState.SetString("PlayModeTest.Result", result);
            SessionState.SetString("PlayModeTest.State", "Done");
            EditorApplication.isPlaying = false;
        }
    }
}
