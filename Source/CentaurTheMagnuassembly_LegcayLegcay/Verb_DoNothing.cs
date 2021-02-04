using Verse;
//using AbilityUser;

namespace CentaurTheMagnuassembly
{
    public class Verb_DoNothing : Verb
    {
        protected override bool TryCastShot()
        {
            //throw new NotImplementedException();
            return false;
        }
    }

}
