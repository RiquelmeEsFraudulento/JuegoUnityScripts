/*
using UnityEngine;
using YamlDotNet.Serialization;
using UnityEditor.Animations;
using UnityEngine.Animations;

public class YAML : MonoBehaviour
{
    public TextAsset yamlData;
    public Animator animator;

    void Start()
    {
        if (yamlData != null)
        {
            string yamlText = yamlData.text;
            var deserializer = new DeserializerBuilder().Build();
            var animatorControllerInfo = deserializer.Deserialize<YamlAnimatorController>(yamlText);
            ApplyAnimatorController(animatorControllerInfo);
        }
    }

    private AnimatorState FindState(AnimatorStateMachine stateMachine, string stateName)
    {
        foreach (var state in stateMachine.states)
        {
            if (state.state != null && state.state.name == stateName)
            {
                return state.state;
            }
        }

        return null;
    }



    private void ApplyAnimatorController(YamlAnimatorController animatorControllerInfo)
    {
        if (animatorControllerInfo != null && animator != null)
        {
            var animatorController = new AnimatorController();
            animatorController.name = animatorControllerInfo.m_Name;

            foreach (var layerData in animatorControllerInfo.m_AnimatorLayers)
            {
                var animatorLayer = new AnimatorControllerLayer();
                animatorLayer.name = layerData.m_Name;

                var stateMachine = new AnimatorStateMachine();
                stateMachine.name = layerData.m_StateMachine.m_Name;

                foreach (var stateData in layerData.m_StateMachine.m_States)
                {
                    var state = stateMachine.AddState(stateData.m_Name);
                    state.speed = stateData.m_Speed;
                    state.cycleOffset = stateData.m_CycleOffset;

                    foreach (var transitionData in stateData.m_Transitions)
                    {
                        var transition = state.AddTransition(FindState(stateMachine, transitionData.m_DstState));
                        transition.exitTime = transitionData.m_ExitTime;
                        transition.hasExitTime = transitionData.m_HasExitTime;
                    }

                    state.motion = Resources.Load<Motion>("Animations/" + stateData.m_Motion.m_Name);
                }

                animatorLayer.stateMachine = stateMachine;
                animatorController.AddLayer(animatorLayer);
            }

            animator.runtimeAnimatorController = animatorController;
        }
    }

    [System.Serializable]
    public class YamlAnimatorController
    {
        public string m_Name;
        public YamlAnimatorLayer[] m_AnimatorLayers;
    }

    [System.Serializable]
    public class YamlAnimatorLayer
    {
        public string m_Name;
        public YamlStateMachine m_StateMachine;
    }

    [System.Serializable]
    public class YamlStateMachine
    {
        public string m_Name;
        public YamlState[] m_States;
    }

    [System.Serializable]
    public class YamlState
    {
        public string m_Name;
        public float m_Speed;
        public float m_CycleOffset;
        public YamlTransition[] m_Transitions;
        public YamlMotion m_Motion;
    }

    [System.Serializable]
    public class YamlTransition
    {
        public string m_DstState;
        public float m_ExitTime;
        public bool m_HasExitTime;
    }

    [System.Serializable]
    public class YamlMotion
    {
        public string m_Name;
    }
}

*/