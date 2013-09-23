using System;
using Aga.Controls.Tree;

namespace VersionOne.VisualStudio.VSPackage.Controls
{
  public static class AdvTreeViewExtensions
  {
    public static TreeNodeAdv FindNodeByMather(this TreeViewAdv node, Predicate<TreeNodeAdv> matcher)
    {
      return FindNodeByMather(node.Root, matcher);
    }

    private static TreeNodeAdv FindNodeByMather(TreeNodeAdv root, Predicate<TreeNodeAdv> matcher)
    {
      foreach (var adv in root.Children)
      {
        if (matcher(adv))
        {
          return adv;
        }
        var adv2 = FindNodeByMather(adv, matcher);
        if (adv2 != null)
        {
          return adv2;
        }
      }
      return null;
    }
  }
}