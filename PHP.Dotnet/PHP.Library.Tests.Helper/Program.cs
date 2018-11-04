﻿using System;
using Xunit;

namespace PHP.Library.Tests.Helper
{
    class Program
    {
        static void Main (string [] args)
        {
            Console.WriteLine ("Hello World!");

            new ProgramTests ().Main ();
        }
    }
}


public class ProgramTests : BaseTests
{

    public void Main1 ()
    {
        string _defs = @"
            <?php
            class B {
                private function myfunc2 ($p1, $p2)
                {
                    return 'base('.$p1.','.$p2.')';
                }
            }
            class A extends B
            {
                private $priv_att = 4;
                public $pub_att = 5;
                public static $stat_pub_att = 6;
                private function myfunc2 ($p1, $p2)
                {
                    echo parent::myfunc2($p1, $p2) . $p1 . $p2;
                }
                function myfunc1 ($p1, $p2)
                {
                    echo 1;
                    return 2;
                }
            }
            $zzz_is_set = isset($zzz);
            while (true) { break; }
        ";

        _eval (_defs, o => o.DEBUG_EXECUTION = true);

        Assert.Equal ("2", _eval (_defs + " $a = new A(); $a->myfunc1(9,9.5); ", o => o.DEBUG_EXECUTION = true));

        Assert.Equal ("base(9,9.5)99.5", _eval (_defs + " $a = new A(); $a->myfunc2(9,9.5); ", o => o.DEBUG_EXECUTION = true));



        //      Assert.Equal ("3", _eval (defs + ""));
    }

    public void Main ()
    {

        string _defs = @"
            <?php
            $var1 = 'fck';
            $a1 = array( 'key1' => 'bla', 'key2' => 'blabla', 'key3' => $var1, );
            $a2 = array( 'key1' => $a1['key2'], 'key2' => $a1['key1'] );
            $a2['key3'] = 'zzz';
            $a2[] = 'zzz';
        ";

        _eval (_defs, o => o.DEBUG_EXECUTION = true);
        Assert.Equal ("1", _eval (_defs + " $a1['key1'] == $a2['key2'] ? 1 : 0 ", o => o.DEBUG_EXECUTION = true));



    }
}
