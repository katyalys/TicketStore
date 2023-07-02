namespace Order.Domain.ErrorModels
{
    public class Result<T> : Result
    {
        public T Value { get; set; }
    }

    public class Result
    {
        public bool Succeeded => !Errors.Any();
        public List<ErrorClass> Errors { get; set; } = new List<ErrorClass>();
    }
}
