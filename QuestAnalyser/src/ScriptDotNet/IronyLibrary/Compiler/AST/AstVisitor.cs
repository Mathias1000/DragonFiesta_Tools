namespace Irony.Compiler
{
    public interface IAstVisitor
    {
        void BeginVisit(AstNode node);

        void EndVisit(AstNode node);
    }
}