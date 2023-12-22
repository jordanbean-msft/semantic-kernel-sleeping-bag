import * as React from "react";
import mermaid from "mermaid";
import { ResponseMessage } from "../../@types/ResponseMessage";

mermaid.initialize({
  startOnLoad: true,
});

interface ResponseGraphProps {
    response: ResponseMessage | undefined
}

export default function ResponseGraph({ response }: ResponseGraphProps) {
    let data = ConvertToMermaidDiagram(response);

    React.useEffect(() => {
        mermaid.initialize({ sequence: { showSequenceNumbers: true } });
        mermaid.contentLoaded();
    }, []);

    return (
    <div className="mermaid">
        { data }         
    </div>
  );
}

function ConvertToMermaidDiagram(response: ResponseMessage | undefined) {
    let chatMessages = "";
    let pluginFunctionNames: Set<string> = new Set<string>();
    let semanticFunctionNames: Set<string> = new Set<string>();
    let pluginFunctions = "";
    let semanticFunctions = "";

    response?.chatHistory.forEach((message) => {
        if((message.role === "assistant" && message.functionName !== "" && !message.functionName.includes("_")) || message.functionName === "UserInteraction_SendFinalAnswer") {
            semanticFunctionNames.add(message.functionName);
        }

        if (message.role === "assistant" && message.functionName !== "" && message.functionName.includes("_") && message.functionName !== "UserInteraction_SendFinalAnswer") {
            pluginFunctionNames.add(message.functionName);
        }

    });

    pluginFunctionNames.forEach((functionName) => {
        pluginFunctions += `    participant ${functionName}\n`;
    });

    semanticFunctionNames.forEach((functionName) => {
        semanticFunctions += `    participant ${functionName}\n`;
    });

    response?.chatHistory.forEach((message, index, chatHistory) => {
        if (message.role === "system") {
            chatMessages += `    OpenAI->>WebApp: ${addNewLineAt80Characters(`${message.content?.replace(/(?:\r\n|\r|\n)/g, '<br/>').trimEnd()}`)}\n`;
            chatMessages += `    Note over OpenAI,WebApp: ${message.role}\n`
        }

        if (message.role === "assistant") {
            if (message.functionName !== "") {
                chatMessages += `    OpenAI->>WebApp: ${addNewLineAt80Characters(`${message.functionName} - ${message.functionArguments.replace(/(?:\r\n|\r|\n)/g, '')}`)}\n`;
                if (message.totalTokens > 0) {
                    chatMessages += `    Note over OpenAI,WebApp: ${message.role} - P:${message.promptTokens}/C:${message.completionTokens}/T:${message.totalTokens}\n`
                }
                chatMessages += `    WebApp->>${message.functionName}: ${addNewLineAt80Characters(`${message.functionArguments.replace(/(?:\r\n|\r|\n)/g, '')}`)}\n`;
                chatMessages += `    Note over WebApp,${message.functionName}: ${message.role}\n`
            }
            else {
                chatMessages += `    OpenAI->>WebApp: ${addNewLineAt80Characters(`${message.content?.replace(/(?:\r\n|\r|\n)/g, '<br/>')}`)}\n`;
                if (message.totalTokens > 0) {
                    chatMessages += `    Note over OpenAI,WebApp: ${message.role} - P:${message.promptTokens}/C:${message.completionTokens}/T:${message.totalTokens}\n`
                }
            }
        }

        if (message.role === "user") {
            chatMessages += `    WebApp->>WebApp: ${addNewLineAt80Characters(`${message.content?.replace(/(?:\r\n|\r|\n)/g, '<br/>')}`)}\n`;
            chatMessages += `    Note over WebApp,WebApp: ${message.role}\n`;
        }

        if (message.role === "tool") {
            chatMessages += `    ${chatHistory[index - 1].functionName}->>WebApp: ${addNewLineAt80Characters(`${message.content?.replace(/(?:\r\n|\r|\n)/g, '<br/>')}`)}\n`;
            chatMessages += `    Note over ${chatHistory[index - 1].functionName},WebApp: ${message.role}\n`;
            chatMessages += `    WebApp->>OpenAI: ${addNewLineAt80Characters(`${chatHistory[index - 1].functionName} ${message.content?.replace(/(?:\r\n|\r|\n)/g, '<br/>')}`)}\n`;
            chatMessages += `    Note over WebApp,OpenAI: ${message.role}\n`;
        }
    }
    );
   
    let mermaidDiagram =
    `sequenceDiagram
         box OpenAI Semantic Functions
         ${semanticFunctions}
         participant OpenAI
         end
         participant WebApp
         box Native Plugins
         ${pluginFunctions}
         end
         ${chatMessages}

         `;

    return mermaidDiagram;
}

function addNewLineAt80Characters(input: string) {
    let result = '';
    for (let i = 0; i < input.length; i++) {
        if (i > 0 && i % 80 === 0) {
            result += '<br/>'
        }

        result += input[i];
    }
    return result;
}
