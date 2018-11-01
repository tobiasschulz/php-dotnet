using System;
using Xunit;

public class ObjectTests : BaseTests
{
    [Fact]
    public void ObjectDeclTests ()
    {
        _eval ("<?php class A { private $privatt=4; public $pubatt =5; public static $statpubatt=6; private function myfunc2($p1, $p2) { echo $p1 . $p2; } function myfunc1($p1, $p2) { echo 1; } }", o => o.DEBUG_EXECUTION = true);

        Assert.Equal ("3", _eval ("<?php class A { function myfunc($p1, $p2) { echo 1; } }"));
    }

}
