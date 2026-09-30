# Physics Chamber

A compact Unity 6.3 LTS (6000.3.24f1) physics maze. Open `Assets/Scenes/PhysicsChamber.unity` and press Play. Use **WASD** to steer the blue actor, touch the cyan sensor plate in the starting lane, then push the amber payload around the winding route and into the green deposit pad. The red magnetic gate slides upward after the actor activates the sensor. The in-game HUD shows the current objective, and the Console records the unlock and successful deposit time.

If the scene is missing after cloning, open the project in Unity; you can also use **Chamber → Build Physics Chamber Scene**. The builder creates the scene and adds it to Build Settings.

## Implementation notes

- `ActorController` reads keyboard input in `Update` and applies movement/braking forces in `FixedUpdate`.
- `UnlockSensor` only accepts an object with `ActorController`; the payload cannot unlock the gate by rolling over the plate.
- `SlidingGate` moves the physical gate and its collider upward after `ChamberGameManager.GateUnlocked` becomes true.
- `DepositZone` passes trigger contacts to `ChamberGameManager`. Only `PayloadMarker` objects count, and success is rejected until the gate is unlocked.
- The actor has mass 1.2 and low-friction material; the payload has mass 3.8 and a grippier material; the floor uses higher friction. These differences make the payload slower to redirect and let the actor glide slightly.

## Reflection

### 1. What must be true for a run to count as success? What could a player try to cheat?

The amber cube must physically enter the green Deposit Zone so its Rigidbody collider overlaps the trigger, and it must carry the `PayloadMarker` component. The game manager must already have `GateUnlocked` set, which only happens when an object with `ActorController` enters the cyan sensor trigger. A player might try to roll the payload onto the sensor instead of steering the actor there; `UnlockSensor` checks for `ActorController`, so that does not open the gate. The deposit trigger also rejects the actor and any other object, and an early payload entry only logs that the gate must be unlocked first.

### 2. Describe one unexpected behavior and what caused it.

The payload initially looked as if it could take a shortcut through the gate because the gate was narrower than the center corridor. That left small physical gaps at both sides, so the blocker did not actually close the route. I widened the gate collider to overlap the corridor boundary walls and made its caution stripe a visual child without a second collider. The gate now closes the full lane and lifts with its collision shape after the sensor is activated.

### 3. Pick one design decision. What did you optimize for, and what did you give up?

I made the gate a moving physical object rather than a visual-only unlock, so the sensor changes the space the actor and payload can actually occupy. I optimized for a clear, visible cause and effect and for making the roadblock meaningful to the push task. The tradeoff is that the gate lift is a simple scripted translation rather than a hinge or a physically simulated mechanism, so it looks more like a clean arcade puzzle than a mechanical door.

### 4. What is the weakest part of the system, and what would you change first?

The weakest part is control feel: the actor is a rolling sphere with direct planar acceleration, so it can feel more like steering a puck than guiding a character. I would first tune acceleration, braking, and the actor's drag while playing the scene with a controller and keyboard. That would preserve the simple Rigidbody force model while making it easier to line up the payload at the narrow turns. I would also add a restart prompt after success so repeated attempts are easier to compare.
