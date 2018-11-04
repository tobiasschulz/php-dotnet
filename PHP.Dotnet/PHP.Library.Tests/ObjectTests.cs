using System;
using Xunit;

public class ObjectTests : BaseTests
{
    private readonly string _defs;

    public ObjectTests ()
    {
        _defs = @"
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
                    echo (parent::myfunc2($p1, $p2) . $p1 . $p2);
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
    }

    [Fact]
    public void ObjectDeclTests1 ()
    {
        _eval (_defs);
        Assert.Equal ("2", _eval (_defs + " $a = new A(); $a->myfunc1(9,9.5); "));
    }

    [Fact]
    public void ObjectDeclTests2 ()
    {
        Assert.Equal ("base(9,9.5)99.5", _eval (_defs + " $a = new A(); $a->myfunc2(9,9.5); "));

    }

}
