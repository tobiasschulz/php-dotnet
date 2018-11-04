using System;
using Xunit;

public class ObjectTests : BaseTests
{
    [Fact]
    public void ObjectDeclTests ()
    {
        string defs = @"
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
                    echo parent::myfunc($p1, $p2) . $p1 . $p2;
                }
                function myfunc1 ($p1, $p2)
                {
                    echo 1;
                    return 2;
                }
            }
            while (true) { break; }
        ";
        _eval (defs, o => o.DEBUG_EXECUTION = true);

        _eval (defs + " $a = new A(); $a->myfunc1(9,9.5); ", o => o.DEBUG_EXECUTION = true);


        //      Assert.Equal ("3", _eval (defs + ""));
    }

}
