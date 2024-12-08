using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMetaverse;

namespace SecondLifeBot
{
    public class Movement
    {
        private readonly GridClient _client;
        private static readonly HashSet<UUID> LindenAnimations = new HashSet<UUID>
        {
            Animations.STAND,
            Animations.WALK,
            Animations.RUN,
            Animations.JUMP,
            Animations.SIT,
            Animations.SIT_GROUND,
            Animations.HOVER,
            Animations.HOVER_DOWN,
            Animations.HOVER_UP
        };

        private const float ArrivalThreshold = 2.0f;
        private const int DelayBetweenSteps = 100;

        public Movement(GridClient client)
        {
            _client = client;
        }

        public async Task StartManualMovement(Vector3 targetPosition, bool deanimate = false)
        {

            lock (_client.Self.Movement)
            {
                if (_client.Self.Movement.SitOnGround || !_client.Self.SittingOn.Equals(0))
                {
                    _client.Self.Stand();
                }

                if (deanimate)
                {
                    foreach (var animation in _client.Self.SignaledAnimations.Keys)
                    {
                        if (!LindenAnimations.Contains(animation))
                        {
                            _client.Self.AnimationStop(animation, true);
                        }
                    }
                }

                _client.Self.Movement.AtPos = true;
            }

            await MoveTowardsTargetAsync(targetPosition);
        }

        private async Task MoveTowardsTargetAsync(Vector3 targetPosition)
        {
            while (Vector3.Distance(_client.Self.SimPosition, targetPosition) > ArrivalThreshold)
            {
                AdjustFacingDirection(targetPosition);

                Vector3 stepDirection = Vector3.Normalize(targetPosition - _client.Self.SimPosition) * 0.5f;
                Vector3 nextPosition = _client.Self.SimPosition + stepDirection;

                if (IsPositionValid(nextPosition))
                {
                    _client.Self.Movement.AtPos = true;
                    _client.Self.Movement.SendUpdate();
                }
                else
                {
                    Logger.C("Invalid position detected. Adjusting path...", Logger.MessageType.Warn);
                }

                await Task.Delay(DelayBetweenSteps);
            }

            _client.Self.Movement.AtPos = false;
            _client.Self.Movement.SendUpdate();
        }

        private void AdjustFacingDirection(Vector3 targetPosition)
        {
            _client.Self.Movement.TurnToward(targetPosition);
        }

        private bool IsPositionValid(Vector3 position)
        {
            const float regionSize = 256.0f;
            return position.X >= 0 && position.X < regionSize &&
                   position.Y >= 0 && position.Y < regionSize;
        }

        public void StopMovement()
        {
            Logger.C("Stopping movement.", Logger.MessageType.Warn);

            lock (_client.Self.Movement)
            {
                _client.Self.Movement.AtPos = false;
                _client.Self.Movement.SendUpdate();
            }

            Logger.C("Movement stopped.", Logger.MessageType.Warn);
        }
    }
}
