using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UGP
{
    public static class RandomUserNames
    {
        #region RandomNames
        public static List<string> UserNames = new List<string>()
        {
            "oprah_wind_fury",
            "yes_u_suck",
            "godfather_part_4",
            "itchy_and_scratchy",
            "dumbest_man_alive",
            "google_me_now",
            "i_love_my_mommy",
            "buh-buh-bacon",
            "dexter_morgan",
            "username_copied",
            "twit_twit_facebook",
            "fluffy_rabid",
            "regina_phalange",
            "hairy_poppins",
            "rambo_was_real",
            "lol_i_kill_u fizzy_bubblech",
            "whos_ur_buddha",
            "2_puppies",
            "chemo_therapy",
            "a_cuddly_puppy",
            "chuck_norris",
            "herpes_free_since_03",
            "i_boop_ur_nose",
            "crispy_lips",
            "musty_elbow",
            "iucking_fdiot",
            "u_r_grounded",
            "jelly_butt",
            "not_james_bond",
            "wallet_and_purse",
            "my_name_is",
            "ashley_said_what",
            "can_dice",
            "like_my_likes",
            "image_not_uploaded",
            "unfriend_now",
            "pixie_dust",
            "strike_u_r_out",
            "uncommon_name",
            "been_there_done_that",
            "who_am_i",
            "status_update",
            "me_for_president",
            "real_name_hidden",
            "sick_no_more",
            "1_wish_genie",
            "wanton_butt",
        }; 
        #endregion

        public static string GetUsername()
        {
            var rand = Random.Range(0, UserNames.Count);
            return UserNames[rand];
            Debug.Log("AssignedName");
        }

        public static Color GetColor()
        {
            return Random.ColorHSV(0, 255);
            Debug.Log("AssignedColor");
        }
    }
}