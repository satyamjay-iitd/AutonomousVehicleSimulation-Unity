using System.Collections.Generic;

namespace Mapping.OrbSlam
{
    public class OrbSlamOutput
    {
       // public IReadOnlyList<IReadOnlyList<float>> Pose  { get; }
        public IReadOnlyList<float> Pose  { get; }

        public OrbSlamOutput(IReadOnlyList<float> pose = null)
        {
            Pose = pose;
        }

        // public override string ToString()
        // {
        //     return (Pose[0][0] + " " + Pose[0][1] + " " + Pose[0][2] + " " + Pose[0][3] + ", "+
        //             Pose[1][0] + " " + Pose[1][1] + " " + Pose[1][2] + " " + Pose[1][3] + ", " +
        //             Pose[2][0] + " " + Pose[2][1] + " " + Pose[2][2] + " " + Pose[2][3] + ", " +
        //             Pose[3][0] + " " + Pose[3][1] +  " " +Pose[3][2] + " " + Pose[3][3]
        //         );
        // }  
        
        public override string ToString()
        {
            return (Pose[0] + " " + Pose[1] + " " + Pose[2]);
        }  
    }
}