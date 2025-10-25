using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RollbackSandbox
{
    public class GameLogic
    {
        float testSpeed = 300.0f;

        public GameLogic() 
        {

        }

        public void Update(ref GameState currentGameState, GameInput input1, GameInput input2) 
        {

            if(input1 == GameInput.Up) 
            {
                currentGameState.Position1.Y += testSpeed * (1.0f / 60.0f);
            }
            else if(input1 == GameInput.Down) 
            {
                currentGameState.Position1.Y -= testSpeed * (1.0f / 60.0f);
            }
            else if(input1 == GameInput.Right) 
            {
                currentGameState.Position1.X += testSpeed * (1.0f / 60.0f);
            }
            else if(input1 == GameInput.Left) 
            {
                currentGameState.Position1.X -= testSpeed * (1.0f / 60.0f);
            }


            if (input2 == GameInput.Up)
            {
                currentGameState.Position2.Y += testSpeed * (1.0f / 60.0f);
            }
            else if (input2 == GameInput.Down)
            {
                currentGameState.Position2.Y -= testSpeed * (1.0f / 60.0f);
            }
            else if (input2 == GameInput.Right)
            {
                currentGameState.Position2.X += testSpeed * (1.0f / 60.0f);
            }
            else if (input2 == GameInput.Left)
            {
                currentGameState.Position2.X -= testSpeed * (1.0f / 60.0f);
            }

            Debug.WriteLine(input1);
            Debug.WriteLine(input2);
            Debug.WriteLine("Updated the State :)");
        }
    }
}
