using Godot;
using System.Collections.Generic;

public class TreeNode<T>
{
	private T data;
	private List<TreeNode<T>> children;

	public T Data
	{
		get { return data; }
		set { data = value; }
	}

	public TreeNode(T data)
	{
		children = new List<TreeNode<T>>();
		this.data = data;
	}

	public void addChild(TreeNode<T> child)
	{
		children.Add(child);
	}

	public List<T> getChildrenData()
	{
		List<T> childrenData = new List<T>();
		foreach(TreeNode<T> child in children)
		{
			childrenData.Add(child.data);
		}
		return childrenData;
	}

	public List<TreeNode<T>> getChildren()
	{
		return children;
	}
}
