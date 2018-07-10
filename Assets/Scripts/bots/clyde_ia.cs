using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class clyde_ia : MonoBehaviour {

    public Transform[] waypoints;
    private bool firstWaitpoint = false;
    private bool secondWaitpoint = false;
    private Transform backActualWaypoint;
    private Transform actualWaypoint;
    private MapNode map;
    public float speed = 0.5f;
    public static bool passMuraille = false;
    public int timePassMuraille = 0;

    // Use this for initialization
    void Start () {
        this.map = new MapNode(waypoints, passMuraille);

        backActualWaypoint = waypoints[83];
    }
	
	// Update is called once per frame
	void Update () {
        if (passMuraille)
        {
            timePassMuraille++;
            if (timePassMuraille >= 400)
            {
                passMuraille = false;
                speed = 0.15f;
                this.map = new MapNode(waypoints, passMuraille);
            }
        }
        if (!firstWaitpoint)
        {
            if (transform.position != waypoints[83].position)
            {
                Vector2 p = Vector2.MoveTowards(transform.position, waypoints[83].position, speed);
                GetComponent<Rigidbody2D>().MovePosition(p);

                Vector2 dir = waypoints[83].position - transform.position;
                GetComponent<Animator>().SetFloat("DirX", dir.x);
                GetComponent<Animator>().SetFloat("DirY", dir.y);
            }
            else
            {
                firstWaitpoint = true;
            }

        }
        else
        {
            if (!secondWaitpoint)
            {
                if (transform.position != waypoints[82].position)
                {
                    Vector2 p = Vector2.MoveTowards(transform.position, waypoints[82].position, speed);
                    GetComponent<Rigidbody2D>().MovePosition(p);

                    Vector2 dir = waypoints[82].position - transform.position;
                    GetComponent<Animator>().SetFloat("DirX", dir.x);
                    GetComponent<Animator>().SetFloat("DirY", dir.y);
                }
                else
                {
                    actualWaypoint = waypoints[82];
                    secondWaitpoint = true;
                    Node[] wpns = (Node[])map.GetSuccessors(new WaypointNode("tempo", actualWaypoint));
                    int nbNext = wpns.Count();
                    System.Random aleatoire = new System.Random();
                    int choix = aleatoire.Next(nbNext);
                    actualWaypoint = ((WaypointNode)wpns[choix]).getWaypoint();
                }
            }
            else
            {
                if (transform.position != actualWaypoint.position)
                {
                    Vector2 p = Vector2.MoveTowards(transform.position, actualWaypoint.position, speed);
                    GetComponent<Rigidbody2D>().MovePosition(p);

                    Vector2 direction = actualWaypoint.position - transform.position;
                    GetComponent<Animator>().SetFloat("DirX", direction.x);
                    GetComponent<Animator>().SetFloat("DirY", direction.y);
                }
                else
                {
                    //Récupère les nodes successeurs par rapport au node de départ du chemin
                    Node[] wpns = (Node[])map.GetSuccessors(new WaypointNode("tempo", actualWaypoint));

                    //On récupère le nombre de successeurs
                    int nbNext = wpns.Count();

                    //On choisit un index de successeur au pif
                    int choix = new System.Random().Next(nbNext);

                    //On récupère le Waypoint de destination
                    Transform point = ((WaypointNode)wpns[choix]).getWaypoint();

                    //Si le prédécesseur de ce successeur est = au node de départ du chemin alors c'est que l'IA va faire demi-tour. Donc...
                    if (new WaypointNode("", backActualWaypoint).Equals(new WaypointNode("", ((WaypointNode)wpns[choix]).getWaypoint())))
                    {
                        //On prend l'index du successeur suivant
                        choix++;

                        //Si l'index est >= au nombre de successeurs alors je reprends le premier successeur
                        if (choix >= nbNext) choix = 0;

                        //On récupère le nouveau Waypoint de destination
                        point = ((WaypointNode)wpns[choix]).getWaypoint();
                    }

                    //On enregistre le Waypoint de départ
                    backActualWaypoint = actualWaypoint;

                    //On enregistre le Waypoint d'arrivé
                    actualWaypoint = point;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "pacman")
        {
            if (!passMuraille)
            {
                passMuraille = true;
                this.map = new MapNode(waypoints, passMuraille);
                timePassMuraille = 0;
                speed = 0.13f;
            }
            collision.GetComponent<pacmanPlayer>().healthPoint--;
            Instantiate<pacmanPlayer>(collision.GetComponent<pacmanPlayer>(), new Vector3(14, 14, 1), new Quaternion()).name = "pacman";
            Destroy(collision.gameObject);
        }
    }



    //CLASS
    private abstract class Dijkstra
    {

        public abstract Node[] GetPath();
        public abstract int GetWeight();

    }

    [Serializable]
    private class Curve : Edge
    {

        //CONSTRUCTORS
        public Curve(string name, Node src, Node tar) : base(name, src, tar)
        {

        }

        public Curve(string name, Node src, Node tar, int weigth) : base(name, src, tar, weigth)
        {

        }



        //GETTERS & SETTERS
        public Node GetNodeSource()
        {
            return base.GetNode1();
        }

        public void SetNodeSource(Node n1)
        {
            base.SetNode1(n1);
        }

        public Node GetNodeTarget()
        {
            return base.GetNode2();
        }

        public void SetNodeTarget(Node n2)
        {
            base.SetNode1(n2);
        }



        //METHODE PUBLIC
        public override string ToString()
        {
            return "Curve{" + "name=" + base.GetName() + ", nodeSource=" + base.GetNode1() + ", nodeTarget=" + base.GetNode2() + ", weigth=" + base.GetWeigth() + "}";
        }
    }

    [Serializable]
    private class Edge
    {
        private static int cptInt = 0;

        private int weigth;
        private string name;
        private int id;
        private Node n1;
        private Node n2;



        //CONSTRUCTORS
        public Edge(string name, Node n1, Node n2)
        {
            this.name = name;
            this.n1 = n1;
            this.n2 = n2;
            this.weigth = 1;
            this.id = cptInt++;
        }

        public Edge(string name, Node n1, Node n2, int weigth)
        {
            this.weigth = weigth;
            this.name = name;
            this.n1 = n1;
            this.n2 = n2;
            this.id = cptInt++;
        }



        //GETTERS & SETTERS
        public int GetWeigth()
        {
            return this.weigth;
        }

        public void SetWeigth(int weigth)
        {
            this.weigth = weigth;
        }

        public string GetName()
        {
            return this.name;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public Node GetNode1()
        {
            return this.n1;
        }

        public void SetNode1(Node n1)
        {
            this.n1 = n1;
        }

        public Node GetNode2()
        {
            return this.n2;
        }

        public void SetNode2(Node n2)
        {
            this.n2 = n2;
        }



        //METHODES PUBLICS
        public override int GetHashCode()
        {
            int hash = 3;
            hash = 53 * hash + this.id;
            return hash;
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (GetType() != obj.GetType())
            {
                return false;
            }
            Edge other = (Edge)obj;
            if (this.id != other.id)
            {
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            return "Edge{" + "name=" + name + ", node1=" + n1 + ", node2=" + n2 + ", weigth=" + weigth + "}";
        }
    }

    [Serializable]
    private class Node
    {
        private static int cptInt = 0;
        private string name;
        private int id;



        //CONSTRUCTOR
        public Node(string name)
        {
            this.name = name;
            this.id = cptInt++;
        }



        //GETTERS & SETTERS
        public string GetName()
        {
            return name;
        }

        public void SetName(string name)
        {
            this.name = name;
        }



        //METHODE PUBLICS
        public override string ToString()
        {
            return "Node{" + "name=" + name + '}';
        }

        public override int GetHashCode()
        {
            int hash = 5;
            hash = 37 * hash + this.id;
            return hash;
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (GetType() != obj.GetType())
            {
                return false;
            }
            Node other = (Node)obj;
            return this.id == other.id;
        }

        public Node[] GetPredeccessors(GraphTheory gt)
        {
            return gt.GetPredecessors(this);
        }

        public Node[] GetSuccessors(GraphTheory gt)
        {
            return gt.GetSuccessors(this);
        }

        public int NbInDegrees(GraphTheory gt)
        {
            return GetPredeccessors(gt).Length;
        }

        public int NbOutDegrees(GraphTheory gt)
        {
            return GetSuccessors(gt).Length;
        }
    }

    [Serializable]
    private class WaypointNode : Node
    {
        private Transform waypoint;

        public WaypointNode(string name, Transform waypoint) : base(name)
        {
            this.waypoint = waypoint;
        }

        public Transform getWaypoint()
        {
            return this.waypoint;
        }

        public override int GetHashCode()
        {
            int hash = 5;
            hash = 37 * hash + this.waypoint.GetHashCode();
            return hash;
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (GetType() != obj.GetType())
            {
                return false;
            }
            WaypointNode other = (WaypointNode)obj;
            return this.waypoint == other.waypoint;
        }
    }

    [Serializable]
    private abstract class GraphTheory
    {
        private static int cptInt = 0;
        private int id;
        Node[] listNode;
        Edge[] listEdge;



        //CONSTRUCTOR
        public GraphTheory(Node[] listNode, Edge[] listEdge)
        {
            this.listNode = listNode;
            this.listEdge = listEdge;
            this.id = cptInt++;
        }



        //GETTERS & SETTERS
        public Node[] GetListNode()
        {
            return this.listNode;
        }

        public void SetListNode(Node[] listNode)
        {
            this.listNode = listNode;
        }

        public Edge[] GetListEdge()
        {
            return this.listEdge;
        }

        public void SetListEdge(Edge[] listEdge)
        {
            this.listEdge = listEdge;
        }



        //METHODES PUBLICS
        public override int GetHashCode()
        {
            int hash = 3;
            hash = 29 * hash + this.id;
            return hash;
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (GetType() != obj.GetType())
            {
                return false;
            }
            GraphTheory other = (GraphTheory)obj;
            return this.id == other.id;
        }

        public override string ToString()
        {
            return "GraphTheory{" + "\n\tlistNode=" + ToElementString(listNode) + ",\n\tlistEdge=" + ToElementString(listEdge) + "\n}";
        }

        public abstract Node[] GetSuccessors(Node n);

        public abstract Node[] GetPredecessors(Node n);

        public abstract int NbInDegrees(Node n);

        public abstract int NbOutDegrees(Node n);

        public bool PathExistBetween(Node start, Node end)
        {
            return (GetDijkstra(start, end).GetPath() != null);
        }

        public Dijkstra GetDijkstra(Node start, Node end)
        {
            int[,] mat = GenerateMatrix(listNode, listEdge);
            return new DijkstraObject(mat, IndexOf(listNode, start), IndexOf(listNode, end), listNode);
        }



        //METHODES PRIVATES
        private int IndexOf(Node[] nodes, Node node)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].Equals(node)) return i;
            }
            return -1;
        }

        private int[,] GenerateMatrix(Node[] listNode, Edge[] listEdge)
        {
            int[,] mat = new int[listNode.Length, listNode.Length];

            for (int i = 0; i < listNode.Length; i++)
            {
                Node predecessor = listNode[i];
                for (int j = 0; j < listNode.Length; j++)
                {
                    Node successor = listNode[j];
                    mat[i, j] = -1;
                    for (int k = 0; k < listEdge.Length; k++)
                    {
                        Edge e = listEdge[k];
                        if (e is Curve)
                        {
                            Curve c = (Curve)e;
                            if (c.GetNodeSource().Equals(predecessor) && c.GetNodeTarget().Equals(successor))
                            {
                                mat[i, j] = c.GetWeigth();
                            }
                        }
                        else
                        {
                            if ((e.GetNode1().Equals(predecessor) && e.GetNode2().Equals(successor)) || (e.GetNode1().Equals(successor) && e.GetNode2().Equals(predecessor)))
                            {
                                mat[i, j] = e.GetWeigth();
                                mat[j, i] = e.GetWeigth();
                            }
                        }
                    }
                }
            }
            return mat;
        }



        //METHODE PRIVATE STATIC
        private static string ToElementString<T>(T[] array)
        {
            var builder = new StringBuilder();
            builder.Append('[');
            for (int i = 0; i < array.Length; i++)
            {
                if (i == 0)
                {
                    builder.Append(array[i]);
                }
                else
                {
                    builder.Append(", " + array[i]);
                }
            }
            builder.Append(']');
            return builder.ToString();
        }



        //CLASS
        private class DijkstraObject : Dijkstra
        {

            private int[,] matrice;
            private int[] distanceFromStart;
            private int[] precedences;
            private bool[] activesNodes;
            private int dim;

            private int[] path = null;
            private int weight = -1;
            private Node[] listNode;



            //CONSTRUCTOR
            public DijkstraObject(int[,] matrice, int start, int end, Node[] listNode)
            {
                this.listNode = listNode;
                System.Collections.Generic.List<int> list = GetPath(matrice, start, end);
                if (list != null)
                {
                    if (list.Count() >= 3)
                    {
                        weight = list.ElementAt(list.Count() - 1);
                        path = new int[list.Count() - 1];
                        for (int i = 0; i < list.Count() - 1; i++)
                        {
                            path[i] = list.ElementAt(i);
                        }
                    }
                }
            }



            //METHODES PUBLICS
            public override Node[] GetPath()
            {
                if (path == null) return null;
                Node[] ns = new Node[path.Length];
                for (int i = 0; i < path.Length; i++)
                {
                    int index = path[i];
                    ns[i] = listNode[index];
                }
                return ns;
            }

            public override int GetWeight()
            {
                return weight;
            }



            //METHODES PRIVATES
            private void ActiveAdjacents(int node)
            {
                int distanceTo;
                for (int to = 0; to < this.dim; to++)
                    if (this.IsAdjacent(node, to) && (distanceTo = this.distanceFromStart[node] + this.matrice[node, to]) < this.distanceFromStart[to])
                        this.ActiveNode(node, to, distanceTo);
            }

            private void ActiveNode(int from, int node, int distance)
            {
                this.distanceFromStart[node] = distance;
                this.precedences[node] = from;
                this.activesNodes[node] = true;
            }

            private System.Collections.Generic.List<int> BuildPath(int end)
            {
                System.Collections.Generic.List<int> path = new System.Collections.Generic.List<int>();
                path.Add(end);

                // utilisation d'une boucle do-while pour conserver le point de depart
                // et d'arrivee dans la liste même lorsque le point de depart correspond
                // au point d'arrivee
                int position = end;
                do
                {
                    path.Insert(0, this.precedences[position]);
                    position = path.ElementAt(0);
                } while (this.distanceFromStart[position] != 0);
                if (path.Any())
                {
                    int compteur = 0;
                    for (int i = 0; i < path.Count(); i++)
                    {
                        try
                        {
                            compteur += matrice[path.ElementAt(i), path.ElementAt(i + 1)];
                        }
                        catch (ArgumentOutOfRangeException e) { }
                    }
                    path.Add(compteur);
                }
                return path;
            }

            private System.Collections.Generic.List<int> GetPath(int[,] graph, int start, int end)
            {
                if (start == end)
                {
                    System.Collections.Generic.List<int> l = new System.Collections.Generic.List<int>();
                    l.Add(start);
                    l.Add(end);
                    l.Add(0);
                    return l;
                }

                return this.GetPath(graph, new int[] { start }, new int[] { end });
            }

            private System.Collections.Generic.List<int> GetPath(int[,] graph, int[] starts, int[] ends)
            {
                Array.Sort(ends);

                // initialisation des variables necessaires a la resolution du probleme
                this.Init(graph, starts);

                // calcul des distances par rapport au point de depart et recuperation
                // du point d'arrivee
                int end = this.ProcessDistances(ends);

                return (end != -1) ? this.BuildPath(end) : null;
            }

            private void Init(int[,] graph, int[] start)
            {
                this.matrice = graph;
                this.dim = graph.GetLength(0);
                this.activesNodes = new bool[this.dim];
                this.precedences = new int[this.dim];

                Fill(this.precedences, -1);

                this.distanceFromStart = new int[this.dim];

                Fill(this.distanceFromStart, int.MaxValue);

                foreach (int value in start)
                    this.ActiveNode(value, value, 0);
            }

            private bool IsAdjacent(int from, int to)
            {
                return this.matrice[from, to] >= 0;
            }

            private int ProcessDistances(int[] ends)
            {
                // selectionne le prochain noeud a analyser (noeud courant)
                int next = this.SelectNextNode();

                // return -1 s'il n'y a plus de noeud a analyser, donc qu'il n'existe
                // pas de chemin satisfaisant la recherche
                if (next == -1)
                    return -1;

                // retourne la position du noeud actuel s'il appartient au tableau des
                // destinations possibles
                if (Array.BinarySearch(ends, next) >= 0)
                    return next;

                // active les prochains noeuds a analyser a partir du noeud courant
                this.ActiveAdjacents(next);

                // desactive le noeud courant
                this.activesNodes[next] = false;

                // appel recursif de la methode pour traiter le prochain noeud
                return this.ProcessDistances(ends);
            }

            private int SelectNextNode()
            {
                int nextNode = -1;
                for (int node = 0; node < this.dim; node++)
                    if (this.activesNodes[node] && (nextNode == -1 || this.distanceFromStart[node] < this.distanceFromStart[nextNode]))
                        nextNode = node;
                return nextNode;
            }

            private void Fill(int[] tab, int value)
            {
                for (int i = 0; i < tab.Length; i++)
                {
                    tab[i] = value;
                }
            }
        }
    }

    [Serializable]
    private class Matrix : GraphTheory
    {
        private int[,] mat;



        //CONSTRUCTORS
        public Matrix(Node[] listNode, Edge[] listEdge) : base(listNode, listEdge)
        {
            GenerateMatrix(listNode, listEdge);
        }

        public Matrix(Matrix m) : base(m.GetListNode(), m.GetListEdge())
        {
            GenerateMatrix(m.GetListNode(), m.GetListEdge());
        }

        public Matrix(List l) : base(l.GetListNode(), l.GetListEdge())
        {
            GenerateMatrix(l.GetListNode(), l.GetListEdge());
        }



        //METHODES PUBLICS
        public List getList()
        {
            return new List(base.GetListNode(), base.GetListEdge());
        }

        public override Node[] GetPredecessors(Node n)
        {
            System.Collections.Generic.List<Node> list = new System.Collections.Generic.List<Node>();
            for (int i = 0; i < mat.GetLength(0); i++)
            {
                int val = mat[i, IndexOf(base.GetListNode(), n)];
                if (val > 0)
                {
                    list.Add(base.GetListNode()[i]);
                }
            }
            Node[] ns = new Node[list.Count()];
            for (int i = 0; i < list.Count(); i++)
            {
                ns[i] = list.ElementAt(i);
            }
            return ns;
        }

        public override Node[] GetSuccessors(Node n)
        {
            System.Collections.Generic.List<Node> list = new System.Collections.Generic.List<Node>();
            for (int i = 0; i < mat.GetLength(0); i++)
            {

                int val = mat[IndexOf(base.GetListNode(), n), i];
                if (val > 0)
                {
                    list.Add(base.GetListNode()[i]);
                }
            }
            Node[] ns = new Node[list.Count()];
            for (int i = 0; i < list.Count(); i++)
            {
                ns[i] = list.ElementAt(i);
            }
            return ns;
        }

        public override int NbInDegrees(Node n)
        {
            return GetPredecessors(n).Length;
        }

        public override int NbOutDegrees(Node n)
        {
            return GetSuccessors(n).Length;
        }

        public override string ToString()
        {
            int maxLength = 0;

            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    String value = "N";
                    if (mat[i, j] != -1)
                    {
                        value = "" + mat[i, j];
                    }
                    if (value.Length > maxLength) maxLength = value.Length;
                }
            }

            string str = "Matrix{\n\t";
            str += "  |";
            for (int i = 0; i < base.GetListNode().Length; i++)
            {
                Node n = base.GetListNode()[i];
                String value = GetCompleteValue(n.GetName().Substring(0, 1), maxLength + 1);
                if (i == 0)
                {
                    str += value;
                }
                else
                {
                    str += " " + value;
                }
            }
            str += "\n\t--+";
            for (int i = 0; i < base.GetListNode().Length; i++)
            {
                if (i < base.GetListNode().Length - 1)
                    str += "--";
                else
                    str += "-";
                for (int j = 0; j < maxLength; j++)
                {
                    str += "-";
                }
            }
            str += "\n\t";
            for (int i = 0; i < mat.GetLength(0); i++)
            {
                str += base.GetListNode()[i].GetName().Substring(0, 1) + " | ";
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    String value = "N";
                    if (mat[i, j] != -1) value = "" + mat[i, j];
                    value = GetCompleteValue(value, maxLength);
                    if (j == 0)
                    {
                        str += value;
                    }
                    else
                    {
                        str += ", " + value;
                    }
                }
                if (i < mat.Length - 1) str += "\n\t";
            }
            str += "\n}";
            return str;
        }



        //METHODES PRIVATES
        private void GenerateMatrix(Node[] listNode, Edge[] listEdge)
        {
            this.mat = new int[listNode.Length, listNode.Length];

            for (int i = 0; i < listNode.Length; i++)
            {
                Node predecessor = listNode[i];
                for (int j = 0; j < listNode.Length; j++)
                {
                    Node successor = listNode[j];
                    this.mat[i, j] = -1;
                    for (int k = 0; k < listEdge.Length; k++)
                    {
                        Edge e = listEdge[k];
                        if (e is Curve)
                        {
                            Curve c = (Curve)e;
                            if (c.GetNodeSource().Equals(predecessor) && c.GetNodeTarget().Equals(successor))
                            {
                                this.mat[i, j] = c.GetWeigth();
                            }
                        }
                        else
                        {
                            if ((e.GetNode1().Equals(predecessor) && e.GetNode2().Equals(successor)) || (e.GetNode1().Equals(successor) && e.GetNode2().Equals(predecessor)))
                            {
                                this.mat[i, j] = e.GetWeigth();
                                this.mat[j, i] = e.GetWeigth();
                            }
                        }
                    }
                }
            }
        }

        private string GetCompleteValue(string value, int maxLength)
        {
            int sub = maxLength - value.Length;
            string blank = "";
            for (int i = 0; i < sub; i++)
            {
                blank += " ";
            }
            return blank + value;
        }

        private int IndexOf(Node[] nodes, Node node)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].Equals(node)) return i;
            }
            return -1;
        }
    }

    [Serializable]
    private class List : GraphTheory
    {
        private ListNode[] listNodes;



        //CONSTRUCTOR
        public List(Node[] listNode, Edge[] listEdge) : base(listNode, listEdge)
        {
            GenerateList(listNode, listEdge);
        }

        public List(List l) : base(l.GetListNode(), l.GetListEdge())
        {
            GenerateList(l.GetListNode(), l.GetListEdge());
        }

        public List(Matrix m) : base(m.GetListNode(), m.GetListEdge())
        {
            GenerateList(m.GetListNode(), m.GetListEdge());
        }



        //METHODES PUBLICS
        public Matrix GetMatrix()
        {
            return new Matrix(base.GetListNode(), base.GetListEdge());
        }

        public override Node[] GetSuccessors(Node n)
        {
            for (int i = 0; i < listNodes.Length; i++)
            {
                ListNode ln = listNodes[i];
                if (ln.GetFather().Equals(n))
                {
                    System.Collections.Generic.List<Node> nodes = ln.GetSons();
                    Node[] ns = new Node[nodes.Count()];
                    for (int j = 0; j < nodes.Count(); j++)
                    {
                        ns[j] = nodes.ElementAt(j);
                    }
                    return ns;
                }
            }
            return null;
        }

        public override Node[] GetPredecessors(Node n)
        {
            System.Collections.Generic.List<Node> ps = new System.Collections.Generic.List<Node>();
            for (int i = 0; i < listNodes.Length; i++)
            {
                ListNode ln = listNodes[i];
                if (ln.GetSons().Contains(n))
                    if (!ps.Contains(ln.GetFather()))
                        ps.Add(ln.GetFather());
            }
            Node[] ns = new Node[ps.Count()];
            for (int i = 0; i < ps.Count(); i++)
            {
                ns[i] = ps.ElementAt(i);
            }
            return ns;
        }

        public override int NbInDegrees(Node n)
        {
            return GetPredecessors(n).Length;
        }

        public override int NbOutDegrees(Node n)
        {
            return GetSuccessors(n).Length;
        }

        public override string ToString()
        {
            string str = "List{\n\t";
            for (int i = 0; i < listNodes.Length; i++)
            {
                ListNode ln = listNodes[i];
                str += ln.GetFather() + " -> ";
                System.Collections.Generic.List<Node> ns = ln.GetSons();
                for (int j = 0; j < ns.Count(); j++)
                {
                    if (j == 0)
                    {
                        str += ns.ElementAt(j);
                    }
                    else
                    {
                        str += ", " + ns.ElementAt(j);
                    }
                }
                if (i < listNodes.Length - 1) str += "\n\t";
            }
            str += "\n}";
            return str;
        }



        //PRIVATE CLASS
        private void GenerateList(Node[] listNode, Edge[] listEdge)
        {
            listNodes = new ListNode[listNode.Length];
            for (int i = 0; i < listNode.Length; i++)
            {
                Node n = listNode[i];
                ListNode ln = new ListNode(n);
                foreach (Edge e in listEdge)
                {
                    if (e is Curve)
                    {
                        Curve c = (Curve)e;
                        if (c.GetNodeSource().Equals(n))
                        {
                            ln.AddSon(c.GetNodeTarget());
                        }
                    }
                    else
                    {
                        if (e.GetNode1().Equals(n))
                        {
                            ln.AddSon(e.GetNode2());
                        }
                        else if (e.GetNode2().Equals(n))
                        {
                            ln.AddSon(e.GetNode1());
                        }
                    }
                }
                listNodes[i] = ln;
            }
        }



        //CLASS
        [Serializable]
        private class ListNode
        {

            private Node father;
            private System.Collections.Generic.List<Node> sons;

            public ListNode(Node father)
            {
                this.father = father;
                this.sons = new System.Collections.Generic.List<Node>();
            }

            public Node GetFather()
            {
                return father;
            }

            public void AddSon(Node n)
            {
                this.sons.Add(n);
            }

            public System.Collections.Generic.List<Node> GetSons()
            {
                return sons;
            }

            public override string ToString()
            {
                return "ListNode{" + "father=" + father + ", sons=" + sons + '}';
            }

        }
    }

    private class MapNode
    {
        private Matrix matrix;

        public MapNode(Transform[] waypoints, bool passMuraille)
        {
            WaypointNode wn0;
            WaypointNode wn1;
            WaypointNode wn2;
            WaypointNode wn3;
            WaypointNode wn4;
            WaypointNode wn5;
            WaypointNode wn6;
            WaypointNode wn7;
            WaypointNode wn8;
            WaypointNode wn9;
            WaypointNode wn10;
            WaypointNode wn11;
            WaypointNode wn12;
            WaypointNode wn13;
            WaypointNode wn14;
            WaypointNode wn15;
            WaypointNode wn16;
            WaypointNode wn17;
            WaypointNode wn18;
            WaypointNode wn19;
            WaypointNode wn20;
            WaypointNode wn21;
            WaypointNode wn22;
            WaypointNode wn23;
            WaypointNode wn24;
            WaypointNode wn25;
            WaypointNode wn26;
            WaypointNode wn27;
            WaypointNode wn28;
            WaypointNode wn29;
            WaypointNode wn30;
            WaypointNode wn31;
            WaypointNode wn32;
            WaypointNode wn33;
            WaypointNode wn34;
            WaypointNode wn35;
            WaypointNode wn36;
            WaypointNode wn37;
            WaypointNode wn38;
            WaypointNode wn39;
            WaypointNode wn40;
            WaypointNode wn41;
            WaypointNode wn42;
            WaypointNode wn43;
            WaypointNode wn44;
            WaypointNode wn45;
            WaypointNode wn46;
            WaypointNode wn47;
            WaypointNode wn48;
            WaypointNode wn49;
            WaypointNode wn50;
            WaypointNode wn51;
            WaypointNode wn52;
            WaypointNode wn53;
            WaypointNode wn54;
            WaypointNode wn55;
            WaypointNode wn56;
            WaypointNode wn57;
            WaypointNode wn58;
            WaypointNode wn59;
            WaypointNode wn60;
            WaypointNode wn61;
            WaypointNode wn62;
            WaypointNode wn63;
            WaypointNode wn64;
            WaypointNode wn65;
            WaypointNode wn66;
            WaypointNode wn67;
            WaypointNode wn68;
            WaypointNode wn69;
            WaypointNode wn70;
            WaypointNode wn71;
            WaypointNode wn72;
            WaypointNode wn73;
            WaypointNode wn74;
            WaypointNode wn75;
            WaypointNode wn76;
            WaypointNode wn77;
            WaypointNode wn78;
            WaypointNode wn79;
            WaypointNode wn80;
            WaypointNode wn81;
            WaypointNode wn82;
            WaypointNode wn83;

            if (passMuraille)
            {
                List<Transform> reorg = new List<Transform>();
                for(int i = 0; i < waypoints.Length; i++)
                {
                    reorg.Add(waypoints[i]);
                }
                shuffle(reorg);
                Transform[] copy = new Transform[84];
                for(int i = 0; i < reorg.Count; i++)
                {
                    copy[i] = reorg.ElementAt(i);
                }

                wn0 = new WaypointNode("wn0", copy[0]);
                wn1 = new WaypointNode("wn1", copy[1]);
                wn2 = new WaypointNode("wn2", copy[2]);
                wn3 = new WaypointNode("wn3", copy[3]);
                wn4 = new WaypointNode("wn4", copy[4]);
                wn5 = new WaypointNode("wn5", copy[5]);
                wn6 = new WaypointNode("wn6", copy[6]);
                wn7 = new WaypointNode("wn7", copy[7]);
                wn8 = new WaypointNode("wn8", copy[8]);
                wn9 = new WaypointNode("wn9", copy[9]);
                wn10 = new WaypointNode("wn10", copy[10]);
                wn11 = new WaypointNode("wn11", copy[11]);
                wn12 = new WaypointNode("wn12", copy[12]);
                wn13 = new WaypointNode("wn13", copy[13]);
                wn14 = new WaypointNode("wn14", copy[14]);
                wn15 = new WaypointNode("wn15", copy[15]);
                wn16 = new WaypointNode("wn16", copy[16]);
                wn17 = new WaypointNode("wn17", copy[17]);
                wn18 = new WaypointNode("wn18", copy[18]);
                wn19 = new WaypointNode("wn19", copy[19]);
                wn20 = new WaypointNode("wn20", copy[20]);
                wn21 = new WaypointNode("wn21", copy[21]);
                wn22 = new WaypointNode("wn22", copy[22]);
                wn23 = new WaypointNode("wn23", copy[23]);
                wn24 = new WaypointNode("wn24", copy[24]);
                wn25 = new WaypointNode("wn25", copy[25]);
                wn26 = new WaypointNode("wn26", copy[26]);
                wn27 = new WaypointNode("wn27", copy[27]);
                wn28 = new WaypointNode("wn28", copy[28]);
                wn29 = new WaypointNode("wn29", copy[29]);
                wn30 = new WaypointNode("wn30", copy[30]);
                wn31 = new WaypointNode("wn31", copy[31]);
                wn32 = new WaypointNode("wn32", copy[32]);
                wn33 = new WaypointNode("wn33", copy[33]);
                wn34 = new WaypointNode("wn34", copy[34]);
                wn35 = new WaypointNode("wn35", copy[35]);
                wn36 = new WaypointNode("wn36", copy[36]);
                wn37 = new WaypointNode("wn37", copy[37]);
                wn38 = new WaypointNode("wn38", copy[38]);
                wn39 = new WaypointNode("wn39", copy[39]);
                wn40 = new WaypointNode("wn40", copy[40]);
                wn41 = new WaypointNode("wn41", copy[41]);
                wn42 = new WaypointNode("wn42", copy[42]);
                wn43 = new WaypointNode("wn43", copy[43]);
                wn44 = new WaypointNode("wn44", copy[44]);
                wn45 = new WaypointNode("wn45", copy[45]);
                wn46 = new WaypointNode("wn46", copy[46]);
                wn47 = new WaypointNode("wn47", copy[47]);
                wn48 = new WaypointNode("wn48", copy[48]);
                wn49 = new WaypointNode("wn49", copy[49]);
                wn50 = new WaypointNode("wn50", copy[50]);
                wn51 = new WaypointNode("wn51", copy[51]);
                wn52 = new WaypointNode("wn52", copy[52]);
                wn53 = new WaypointNode("wn53", copy[53]);
                wn54 = new WaypointNode("wn54", copy[54]);
                wn55 = new WaypointNode("wn55", copy[55]);
                wn56 = new WaypointNode("wn56", copy[56]);
                wn57 = new WaypointNode("wn57", copy[57]);
                wn58 = new WaypointNode("wn58", copy[58]);
                wn59 = new WaypointNode("wn59", copy[59]);
                wn60 = new WaypointNode("wn60", copy[60]);
                wn61 = new WaypointNode("wn61", copy[61]);
                wn62 = new WaypointNode("wn62", copy[62]);
                wn63 = new WaypointNode("wn63", copy[63]);
                wn64 = new WaypointNode("wn64", copy[64]);
                wn65 = new WaypointNode("wn65", copy[65]);
                wn66 = new WaypointNode("wn66", copy[66]);
                wn67 = new WaypointNode("wn67", copy[67]);
                wn68 = new WaypointNode("wn68", copy[68]);
                wn69 = new WaypointNode("wn69", copy[69]);
                wn70 = new WaypointNode("wn70", copy[70]);
                wn71 = new WaypointNode("wn71", copy[71]);
                wn72 = new WaypointNode("wn72", copy[72]);
                wn73 = new WaypointNode("wn73", copy[73]);
                wn74 = new WaypointNode("wn74", copy[74]);
                wn75 = new WaypointNode("wn75", copy[75]);
                wn76 = new WaypointNode("wn76", copy[76]);
                wn77 = new WaypointNode("wn77", copy[77]);
                wn78 = new WaypointNode("wn78", copy[78]);
                wn79 = new WaypointNode("wn79", copy[79]);
                wn80 = new WaypointNode("wn80", copy[80]);
                wn81 = new WaypointNode("wn81", copy[81]);
                wn82 = new WaypointNode("wn82", copy[82]);
                wn83 = new WaypointNode("wn83", copy[83]);
            }
            else
            {
                wn0 = new WaypointNode("wn0", waypoints[0]);
                wn1 = new WaypointNode("wn1", waypoints[1]);
                wn2 = new WaypointNode("wn2", waypoints[2]);
                wn3 = new WaypointNode("wn3", waypoints[3]);
                wn4 = new WaypointNode("wn4", waypoints[4]);
                wn5 = new WaypointNode("wn5", waypoints[5]);
                wn6 = new WaypointNode("wn6", waypoints[6]);
                wn7 = new WaypointNode("wn7", waypoints[7]);
                wn8 = new WaypointNode("wn8", waypoints[8]);
                wn9 = new WaypointNode("wn9", waypoints[9]);
                wn10 = new WaypointNode("wn10", waypoints[10]);
                wn11 = new WaypointNode("wn11", waypoints[11]);
                wn12 = new WaypointNode("wn12", waypoints[12]);
                wn13 = new WaypointNode("wn13", waypoints[13]);
                wn14 = new WaypointNode("wn14", waypoints[14]);
                wn15 = new WaypointNode("wn15", waypoints[15]);
                wn16 = new WaypointNode("wn16", waypoints[16]);
                wn17 = new WaypointNode("wn17", waypoints[17]);
                wn18 = new WaypointNode("wn18", waypoints[18]);
                wn19 = new WaypointNode("wn19", waypoints[19]);
                wn20 = new WaypointNode("wn20", waypoints[20]);
                wn21 = new WaypointNode("wn21", waypoints[21]);
                wn22 = new WaypointNode("wn22", waypoints[22]);
                wn23 = new WaypointNode("wn23", waypoints[23]);
                wn24 = new WaypointNode("wn24", waypoints[24]);
                wn25 = new WaypointNode("wn25", waypoints[25]);
                wn26 = new WaypointNode("wn26", waypoints[26]);
                wn27 = new WaypointNode("wn27", waypoints[27]);
                wn28 = new WaypointNode("wn28", waypoints[28]);
                wn29 = new WaypointNode("wn29", waypoints[29]);
                wn30 = new WaypointNode("wn30", waypoints[30]);
                wn31 = new WaypointNode("wn31", waypoints[31]);
                wn32 = new WaypointNode("wn32", waypoints[32]);
                wn33 = new WaypointNode("wn33", waypoints[33]);
                wn34 = new WaypointNode("wn34", waypoints[34]);
                wn35 = new WaypointNode("wn35", waypoints[35]);
                wn36 = new WaypointNode("wn36", waypoints[36]);
                wn37 = new WaypointNode("wn37", waypoints[37]);
                wn38 = new WaypointNode("wn38", waypoints[38]);
                wn39 = new WaypointNode("wn39", waypoints[39]);
                wn40 = new WaypointNode("wn40", waypoints[40]);
                wn41 = new WaypointNode("wn41", waypoints[41]);
                wn42 = new WaypointNode("wn42", waypoints[42]);
                wn43 = new WaypointNode("wn43", waypoints[43]);
                wn44 = new WaypointNode("wn44", waypoints[44]);
                wn45 = new WaypointNode("wn45", waypoints[45]);
                wn46 = new WaypointNode("wn46", waypoints[46]);
                wn47 = new WaypointNode("wn47", waypoints[47]);
                wn48 = new WaypointNode("wn48", waypoints[48]);
                wn49 = new WaypointNode("wn49", waypoints[49]);
                wn50 = new WaypointNode("wn50", waypoints[50]);
                wn51 = new WaypointNode("wn51", waypoints[51]);
                wn52 = new WaypointNode("wn52", waypoints[52]);
                wn53 = new WaypointNode("wn53", waypoints[53]);
                wn54 = new WaypointNode("wn54", waypoints[54]);
                wn55 = new WaypointNode("wn55", waypoints[55]);
                wn56 = new WaypointNode("wn56", waypoints[56]);
                wn57 = new WaypointNode("wn57", waypoints[57]);
                wn58 = new WaypointNode("wn58", waypoints[58]);
                wn59 = new WaypointNode("wn59", waypoints[59]);
                wn60 = new WaypointNode("wn60", waypoints[60]);
                wn61 = new WaypointNode("wn61", waypoints[61]);
                wn62 = new WaypointNode("wn62", waypoints[62]);
                wn63 = new WaypointNode("wn63", waypoints[63]);
                wn64 = new WaypointNode("wn64", waypoints[64]);
                wn65 = new WaypointNode("wn65", waypoints[65]);
                wn66 = new WaypointNode("wn66", waypoints[66]);
                wn67 = new WaypointNode("wn67", waypoints[67]);
                wn68 = new WaypointNode("wn68", waypoints[68]);
                wn69 = new WaypointNode("wn69", waypoints[69]);
                wn70 = new WaypointNode("wn70", waypoints[70]);
                wn71 = new WaypointNode("wn71", waypoints[71]);
                wn72 = new WaypointNode("wn72", waypoints[72]);
                wn73 = new WaypointNode("wn73", waypoints[73]);
                wn74 = new WaypointNode("wn74", waypoints[74]);
                wn75 = new WaypointNode("wn75", waypoints[75]);
                wn76 = new WaypointNode("wn76", waypoints[76]);
                wn77 = new WaypointNode("wn77", waypoints[77]);
                wn78 = new WaypointNode("wn78", waypoints[78]);
                wn79 = new WaypointNode("wn79", waypoints[79]);
                wn80 = new WaypointNode("wn80", waypoints[80]);
                wn81 = new WaypointNode("wn81", waypoints[81]);
                wn82 = new WaypointNode("wn82", waypoints[82]);
                wn83 = new WaypointNode("wn83", waypoints[83]);
            }

            Curve c0 = new Curve("83 --> 82", wn83, wn82);
            Edge e0 = new Edge("82 <=> 11", wn82, wn11);
            Edge e1 = new Edge("11 <=> 17", wn11, wn17);
            Edge e2 = new Edge("17 <=> 76", wn17, wn76);
            Edge e3 = new Edge("76 <=> 77", wn76, wn77);
            Edge e4 = new Edge("77 <=> 34", wn77, wn34);
            Edge e5 = new Edge("34 <=> 27", wn34, wn27);
            Edge e6 = new Edge("27 <=> 82", wn27, wn82);
            Edge e7 = new Edge("77 <=> 70", wn77, wn70);
            Edge e8 = new Edge("70 <=> 57", wn70, wn57);
            Edge e9 = new Edge("57 <=> 56", wn57, wn56);
            Edge e10 = new Edge("56 <=> 69", wn56, wn69);
            Edge e11 = new Edge("69 <=> 76", wn69, wn76);
            Edge e12 = new Edge("11 <=>  8", wn11, wn8);
            Edge e13 = new Edge("8  <=> 15", wn8, wn15);
            Edge e14 = new Edge("15 <=> 13", wn15, wn13);
            Edge e15 = new Edge("13 <=>  5", wn13, wn5);
            Edge e16 = new Edge("5  <=> 12", wn5, wn12);
            Edge e17 = new Edge("12 <=> 30", wn12, wn30);
            Edge e18 = new Edge("30 <=> 21", wn30, wn21);
            Edge e19 = new Edge("21 <=> 31", wn21, wn31);
            Edge e20 = new Edge("31 <=> 32", wn31, wn32);
            Edge e21 = new Edge("32 <=> 24", wn32, wn24);
            Edge e22 = new Edge("24 <=> 27", wn24, wn27);
            Edge e23 = new Edge("30 <=> 18", wn30, wn18);
            Edge e24 = new Edge("18 <=> 19", wn18, wn19);
            Edge e25 = new Edge("19 <=> 22", wn19, wn22);
            Edge e26 = new Edge("22 <=> 31", wn22, wn31);
            Edge e27 = new Edge("12 <=>  2", wn12, wn2);
            Edge e28 = new Edge(" 2 <=>  1", wn2, wn1);
            Edge e29 = new Edge(" 1 <=>  4", wn1, wn4);
            Edge e30 = new Edge(" 4 <=> 13", wn4, wn13);
            Edge e31 = new Edge("22 <=> 25", wn22, wn25);
            Edge e32 = new Edge("25 <=> 28", wn25, wn28);
            Edge e33 = new Edge("28 <=> 34", wn28, wn34);
            Edge e34 = new Edge("13 <=>  4", wn13, wn4);
            Edge e35 = new Edge(" 4 <=>  7", wn4, wn7);
            Edge e36 = new Edge(" 7 <=> 10", wn7, wn10);
            Edge e37 = new Edge("10 <=> 17", wn10, wn17);
            Edge e38 = new Edge("10 <=> 73", wn10, wn73);
            Edge e39 = new Edge("73 <=> 55", wn73, wn55);
            Edge e40 = new Edge("55 <=> 69", wn55, wn69);
            Edge e41 = new Edge("28 <=> 80", wn28, wn80);
            Edge e42 = new Edge("80 <=> 58", wn80, wn58);
            Edge e43 = new Edge("58 <=> 70", wn58, wn70);
            Edge e44 = new Edge(" 1 <=>  0", wn1, wn0);
            Edge e45 = new Edge(" 0 <=>  3", wn0, wn3);
            Edge e46 = new Edge(" 3 <=>  4", wn3, wn4);
            Edge e47 = new Edge("19 <=> 20", wn19, wn20);
            Edge e48 = new Edge("20 <=> 23", wn20, wn23);
            Edge e49 = new Edge("23 <=> 22", wn23, wn22);
            Edge e50 = new Edge(" 3 <=>  6", wn3, wn6);
            Edge e51 = new Edge(" 6 <=> 14", wn6, wn14);
            Edge e52 = new Edge("14 <=>  7", wn14, wn7);
            Edge e53 = new Edge("23 <=> 26", wn23, wn26);
            Edge e54 = new Edge("26 <=> 33", wn26, wn33);
            Edge e55 = new Edge("33 <=> 25", wn33, wn25);
            Edge e56 = new Edge("14 <=> 16", wn14, wn16);
            Edge e57 = new Edge("16 <=>  9", wn16, wn9);
            Edge e58 = new Edge(" 9 <=> 72", wn9, wn72);
            Edge e59 = new Edge("72 <=> 73", wn72, wn73);
            Edge e60 = new Edge("33 <=> 35", wn33, wn35);
            Edge e61 = new Edge("35 <=> 29", wn35, wn29);
            Edge e62 = new Edge("29 <=> 81", wn29, wn81);
            Edge e63 = new Edge("81 <=> 80", wn81, wn80);
            Edge e64 = new Edge("72 <=> 74", wn72, wn74);
            Edge e65 = new Edge("74 <=> 75", wn74, wn75);
            Edge e66 = new Edge("75 <=> 68", wn75, wn68);
            Edge e67 = new Edge("68 <=> 55", wn68, wn55);
            Edge e68 = new Edge("81 <=> 79", wn81, wn79);
            Edge e69 = new Edge("79 <=> 78", wn79, wn78);
            Edge e70 = new Edge("78 <=> 71", wn78, wn71);
            Edge e71 = new Edge("71 <=> 58", wn71, wn58);
            Edge e72 = new Edge("55 <=> 49", wn55, wn49);
            Edge e73 = new Edge("49 <=> 65", wn49, wn65);
            Edge e74 = new Edge("65 <=> 50", wn65, wn50);
            Edge e75 = new Edge("50 <=> 56", wn50, wn56);
            Edge e76 = new Edge("58 <=> 52", wn58, wn52);
            Edge e77 = new Edge("52 <=> 66", wn52, wn66);
            Edge e78 = new Edge("66 <=> 51", wn66, wn51);
            Edge e79 = new Edge("51 <=> 57", wn51, wn57);
            Edge e80 = new Edge("65 <=> 61", wn65, wn61);
            Edge e81 = new Edge("61 <=> 44", wn61, wn44);
            Edge e82 = new Edge("44 <=> 38", wn44, wn38);
            Edge e83 = new Edge("38 <=> 39", wn38, wn39);
            Edge e84 = new Edge("39 <=> 45", wn39, wn45);
            Edge e85 = new Edge("45 <=> 62", wn45, wn62);
            Edge e86 = new Edge("62 <=> 66", wn62, wn66);
            Edge e87 = new Edge("49 <=> 43", wn49, wn43);
            Edge e88 = new Edge("43 <=> 37", wn43, wn37);
            Edge e89 = new Edge("37 <=> 38", wn37, wn38);
            Edge e90 = new Edge("52 <=> 46", wn52, wn46);
            Edge e91 = new Edge("46 <=> 40", wn46, wn40);
            Edge e92 = new Edge("40 <=> 39", wn40, wn39);
            Edge e93 = new Edge("37 <=> 36", wn37, wn36);
            Edge e94 = new Edge("36 <=> 42", wn36, wn42);
            Edge e95 = new Edge("42 <=> 60", wn42, wn60);
            Edge e96 = new Edge("60 <=> 43", wn60, wn43);
            Edge e97 = new Edge("40 <=> 41", wn40, wn41);
            Edge e98 = new Edge("41 <=> 47", wn41, wn47);
            Edge e99 = new Edge("47 <=> 63", wn47, wn63);
            Edge e100 = new Edge("63 <=> 46", wn63, wn46);
            Edge e101 = new Edge("68 <=> 54", wn68, wn54);
            Edge e102 = new Edge("54 <=> 48", wn54, wn48);
            Edge e103 = new Edge("48 <=> 64", wn48, wn64);
            Edge e104 = new Edge("64 <=> 60", wn64, wn60);
            Edge e105 = new Edge("71 <=> 59", wn71, wn59);
            Edge e106 = new Edge("59 <=> 53", wn59, wn53);
            Edge e107 = new Edge("53 <=> 67", wn53, wn67);
            Edge e108 = new Edge("67 <=> 63", wn67, wn63);

            WaypointNode[] nodes = new WaypointNode[] { wn0, wn1, wn2, wn3, wn4, wn5, wn6, wn7, wn8, wn9, wn10, wn11, wn12, wn13, wn14, wn15, wn16, wn17, wn18, wn19, wn20, wn21, wn22, wn23, wn24, wn25, wn26, wn27, wn28, wn29, wn30, wn31, wn32, wn33, wn34, wn35, wn36, wn37, wn38, wn39, wn40, wn41, wn42, wn43, wn44, wn45, wn46, wn47, wn48, wn49, wn50, wn51, wn52, wn53, wn54, wn55, wn56, wn57, wn58, wn59, wn60, wn61, wn62, wn63, wn64, wn65, wn66, wn67, wn68, wn69, wn70, wn71, wn72, wn73, wn74, wn75, wn76, wn77, wn78, wn79, wn80, wn81, wn82, wn83 };
            Edge[] edges = new Edge[] { c0, e0, e1, e2, e3, e4, e5, e6, e7, e8, e9, e10, e11, e12, e13, e14, e15, e16, e17, e18, e19, e20, e21, e22, e23, e24, e25, e26, e27, e28, e29, e30, e31, e32, e33, e34, e35, e36, e37, e38, e39, e40, e41, e42, e43, e44, e45, e46, e47, e48, e49, e50, e51, e52, e53, e54, e55, e56, e57, e58, e59, e60, e61, e62, e63, e64, e65, e66, e67, e68, e69, e70, e71, e72, e73, e74, e75, e76, e77, e78, e79, e80, e81, e82, e83, e84, e85, e86, e87, e88, e89, e90, e91, e92, e93, e94, e95, e96, e97, e98, e99, e100, e101, e102, e103, e104, e105, e106, e107, e108 };

            this.matrix = new Matrix(nodes, edges);
        }

        private void shuffle(IList list)
        {
            int n = list.Count;
            System.Random rnd = new System.Random();
            while (n > 1)
            {
                int k = (rnd.Next(0, n) % n);
                n--;
                object value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public Node[] GetSuccessors(Node node)
        {
            return matrix.GetSuccessors(node);
        }
    }
}
