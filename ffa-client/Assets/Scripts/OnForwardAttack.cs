using UnityEngine;

public class OnForwardAttack : StateMachineBehaviour {

    public PlayerMotor playerMotor;

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (playerMotor != null) playerMotor.ResetIsAttacking();
	}
}
