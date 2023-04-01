namespace University.Server.Domain.Models
{
    public class CalculationRow<T1, T2> where T1 : Base where T2 : Base
    {
        public T1 Target { get; set; }
        public ICollection<T2> Available { get; set; }
        public ICollection<T2> Assigned { get; set; }

        public bool TryAssign(T2 assignableObject, Func<CalculationRow<T1, T2>, T2, bool> condition)
        {
            if (Available.Contains(assignableObject) && condition(this, assignableObject))
            {
                Assigned.Add(assignableObject);
                return true;
            }
            return false;
        }

        public bool TryAssign(T2 assignableObject)
        {
            if (Available.Contains(assignableObject))
            {
                Assigned.Add(assignableObject);
                return true;
            }
            return false;
        }

        public bool AddAvailable(T2 assignableObject)
        {
            if (!Available.Contains(assignableObject))
            {
                Available.Add(assignableObject);
                return true;
            }
            return false;
        }

        public CalculationRow(T1 assignableOwner, ICollection<T2> assignableObjects)
        {
            Target = assignableOwner;
            Available = assignableObjects;
            Assigned = new List<T2>();
        }
    }
}
