namespace Assembler{

public static class Assembler{

    public static byte[] compile(string linesOfCode, bool checkSyntax=true){
        
        byte[] r = new byte[0];
        
        // check syntax first
        if(checkSyntax){
            string syntaxErrors = SyntaxChecker.evaluateProgram(linesOfCode);
            if(syntaxErrors != "") return r;
        }

        string derivedVer = PreprocessorDirectives.translateAlias(linesOfCode);

        return Translator.translateProgram(derivedVer);
    }

}

}