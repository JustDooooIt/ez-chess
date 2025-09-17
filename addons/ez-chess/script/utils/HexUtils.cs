// HexUtils.cs
using Godot;
using System; // Required for Math.Abs

public static class HexUtils
{
  public enum HexLayout
  {
    Pointy,
    Flat
  }

  private static Vector3I OffsetToCubePointy(Vector2I cell)
  {
    int q = cell.X - (cell.Y - (cell.Y & 1)) / 2;
    int r = cell.Y;
    int s = -q - r;
    return new Vector3I(q, r, s);
  }

  private static Vector3I OffsetToCubeFlat(Vector2I cell)
  {
    int q = cell.X;
    int r = cell.Y - (cell.X - (cell.X & 1)) / 2;
    int s = -q - r;
    return new Vector3I(q, r, s);
  }

  private static int CubeDistance(Vector3I aCube, Vector3I bCube)
  {
    Vector3I diff = aCube - bCube;
    return (Math.Abs(diff.X) + Math.Abs(diff.Y) + Math.Abs(diff.Z)) / 2;
  }

  public static int GetHexDistance(Vector2I cellA, Vector2I cellB, HexLayout layout)
  {
    Vector3I aCube;
    Vector3I bCube;

    if (layout == HexLayout.Pointy)
    {
      aCube = OffsetToCubePointy(cellA);
      bCube = OffsetToCubePointy(cellB);
    }
    else
    {
      aCube = OffsetToCubeFlat(cellA);
      bCube = OffsetToCubeFlat(cellB);
    }

    return CubeDistance(aCube, bCube);
  }
}
