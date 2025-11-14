using UnityEngine;

public class BahayKuboTracker : MonoBehaviour
{
    [Header("Part Costs")]
    public int haligi_bambooCost = 16;
    public int sahig_hardwoodCost = 10;
    public int pader_sawaliCost = 12;
    public int bubong_nipaCost = 12;

    private bool haligiDone = false;
    private bool sahigDone = false;
    private bool paderDone = false;
    private bool bubongDone = false;

    public string AttemptBuild(PlayerStats player)
    {
        if (!haligiDone)
        {
            if (player.bamboo >= haligi_bambooCost)
            {
                player.ChangeBamboo(-haligi_bambooCost);
                haligiDone = true;
                return "Haligi (Posts)";
            }
            else
            {
                return "NotEnoughBamboo";
            }
        }
        else if (!sahigDone)
        {
            if (player.hardwood >= sahig_hardwoodCost)
            {
                player.ChangeHardwood(-sahig_hardwoodCost);
                sahigDone = true;
                return "Sahig (Floor)";
            }
            else
            {
                return "NotEnoughHardwood";
            }
        }
        else if (!paderDone)
        {
            if (player.sawali >= pader_sawaliCost)
            {
                player.ChangeSawali(-pader_sawaliCost);
                paderDone = true;
                return "Pader (Walls)";
            }
            else
            {
                return "NotEnoughSawali";
            }
        }
        else if (!bubongDone)
        {
            if (player.nipaLeaves >= bubong_nipaCost)
            {
                player.ChangeNipa(-bubong_nipaCost);
                bubongDone = true;
                return "Bubong (Roof)";
            }
            else
            {
                return "NotEnoughNipa";
            }
        }
        else
        {
            return "Complete";
        }
    }
}