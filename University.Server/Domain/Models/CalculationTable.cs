namespace University.Server.Domain.Models
{
    public class CalculationTable<T1, T2> where T1 : Base where T2 : Base
    {
        public List<CalculationRow<T1, T2>> Rows { get; set; } = new List<CalculationRow<T1, T2>>();

        private bool Assign(T2 assignableObject, Func<CalculationRow<T1, T2>, T2, bool>? condition)
        {
            var sortedRows = Rows.OrderBy(row => row.Assigned.Count)
                                 .ThenBy(row => row.Available.Count)
                                 .ToList();
            foreach (var row in sortedRows)
            {
                if (condition != null
                    ? row.TryAssign(assignableObject, condition)
                    : row.TryAssign(assignableObject))
                {
                    return true;
                }
            }
            return false;
        }

        private bool AssignMany(ICollection<T2> assignableObjects, Func<CalculationRow<T1, T2>, T2, bool>? condition, Func<T2, bool, bool>? onIteration)
        {
            foreach (var assignableObject in assignableObjects)
            {
                bool repeat = false;
                do
                {
                    // try to assign object
                    bool success = Assign(assignableObject, condition);
                    // check if iteration function exists, else just assume that no repeat wanted
                    repeat = onIteration != null ? onIteration(assignableObject, success) : false;
                    // if not successful, and no repeat was request -> cancel
                    if (!success && !repeat)
                    {
                        return false;
                    }
                } while (repeat);
            };
            return true;
        }

        private void InsertRow(T1 assignableOwner, ICollection<T2> assignableObjects)
        {
            Rows.Add(new CalculationRow<T1, T2>(assignableOwner, assignableObjects));
        }

        public bool Calculate(ICollection<T2> assignableObjects, Func<CalculationRow<T1, T2>, T2, bool> condition, Func<T2, bool, bool>? onFail)
        {
            return AssignMany(assignableObjects, condition, onFail);
        }

        public bool Calculate(ICollection<T2> assignableObjects, Func<CalculationRow<T1, T2>, T2, bool> condition)
        {
            return AssignMany(assignableObjects, condition, null);
        }

        public bool Calculate(ICollection<T2> assignableObjects)
        {
            return AssignMany(assignableObjects, null, null);
        }

        public void InsertData(T1 assignableOwner, T2 assignableObject)
        {
            var targetRow = Rows.Find(row => row.Target == assignableOwner);
            if (targetRow == null)
            {
                InsertRow(assignableOwner, new List<T2>() { assignableObject });
            }
            else
            {
                targetRow.AddAvailable(assignableObject);
            }
        }
    }
}
