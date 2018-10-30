using System;
using Xunit;

public class SimpleTests : BaseTests
{
    [Fact]
    public void MathTests ()
    {
        Assert.Equal ("3", _eval ("<?php $a = 1 + 2; "));
        Assert.Equal ("-1", _eval ("<?php $a = 1 - 2; "));
        Assert.Equal ("10", _eval ("<?php $a = 5 * 2; "));
        Assert.Equal ("2.5", _eval ("<?php $a = 5 / 2; "));
    }

    [Fact]
    public void EchoTests ()
    {
        Assert.Equal ("3", _eval ("<?php echo 1 + 2; "));
    }

    [Fact]
    public void IssetTests ()
    {
        Assert.Equal ("3", _eval ("<?php $a = 5; if (isset($a)) echo 3; else echo 4; "));
        Assert.Equal ("4", _eval ("<?php $a = 5; if (isset($b)) echo 3; else echo 4; "));
    }

    [Fact]
    public void DefineTests ()
    {
        Assert.Equal ("3", _eval ("<?php define('a', 5); if (defined('a')) echo 3; else echo 4; "));
        Assert.Equal ("4", _eval ("<?php define('a', 5); if (defined('b')) echo 3; else echo 4; "));
    }

    [Fact]
    public void BoolEvalTests ()
    {
        Assert.Equal ("3", _eval ("<?php $a = 5; if ($a) echo 3; else echo 4; "));
        Assert.Equal ("4", _eval ("<?php $a = 0; if ($a) echo 3; else echo 4; "));
    }

}
