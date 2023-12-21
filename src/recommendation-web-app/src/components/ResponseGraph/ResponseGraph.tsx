import * as React from "react";
import { ResponseMessage } from "../Response/Response";
import mermaid from "mermaid";

mermaid.initialize({
  startOnLoad: true,
});

export default function ResponseGraph({ response }: { response: ResponseMessage }) {
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

function ConvertToMermaidDiagram(response: ResponseMessage) {
    let chatMessages = "";
    let pluginFunctionNames: Set<string> = new Set<string>();
    let semanticFunctionNames: Set<string> = new Set<string>();
    let pluginFunctions = "";
    let semanticFunctions = "";

    response.chatHistory.forEach((message) => {
        if((message.role === "assistant" && message.functionName != "" && !message.functionName.includes("_")) || message.functionName == "UserInteraction_SendFinalAnswer") {
            semanticFunctionNames.add(message.functionName);
        }

        if (message.role === "assistant" && message.functionName != "" && message.functionName.includes("_") && message.functionName != "UserInteraction_SendFinalAnswer") {
            pluginFunctionNames.add(message.functionName);
        }

    });

    pluginFunctionNames.forEach((functionName) => {
        pluginFunctions += `    participant ${functionName}\n`;
    });

    semanticFunctionNames.forEach((functionName) => {
        semanticFunctions += `    participant ${functionName}\n`;
    });

    response.chatHistory.forEach((message, index, chatHistory) => {
        if (message.role === "system") {
            chatMessages += `    WebApp->>OpenAI: ${addNewLineAt80Characters(`${message.role} - ${message.content?.replace(/(?:\r\n|\r|\n)/g, '<br/>').trimEnd()}`)}\n`;
        }

        if (message.role === "assistant") {
            if (message.functionName != "") {
                chatMessages += `    OpenAI->>WebApp: ${addNewLineAt80Characters(`${message.role} - ${message.functionName} - ${message.functionArguments.replace(/(?:\r\n|\r|\n)/g, '')}`)}\n`;
                if (message.totalTokens > 0) {
                    chatMessages += `    Note left of OpenAI: P:${message.promptTokens}/C:${message.completionTokens}/T:${message.totalTokens}\n`
                }
                chatMessages += `    WebApp->>+${message.functionName}: ${addNewLineAt80Characters(`${message.role} - ${message.functionArguments.replace(/(?:\r\n|\r|\n)/g, '')}`)}\n`;
            }
            else {
                chatMessages += `    WebApp->>OpenAI: ${addNewLineAt80Characters(`${message.role} - ${message.content?.replace(/(?:\r\n|\r|\n)/g, '<br/>')}`)}\n`;
                if (message.totalTokens > 0) {
                    chatMessages += `    Note right of OpenAI: P:${message.promptTokens}/C:${message.completionTokens}/T:${message.totalTokens}\n`
                }
            }
        }

        if (message.role === "user") {
            chatMessages += `    WebApp->>WebApp: ${addNewLineAt80Characters(`${message.role} - ${message.content?.replace(/(?:\r\n|\r|\n)/g, '<br/>')}`)}\n`;
        }

        if (message.role === "tool") {
            chatMessages += `    ${chatHistory[index - 1].functionName}->>-WebApp: ${addNewLineAt80Characters(`${message.role} - ${message.content?.replace(/(?:\r\n|\r|\n)/g, '<br/>')}`)}\n`;
            chatMessages += `    WebApp->>OpenAI: ${addNewLineAt80Characters(`${message.role} - ${chatHistory[index - 1].functionName} ${message.content?.replace(/(?:\r\n|\r|\n)/g, '<br/>')}`)}\n`;
        }
    }
    );
   
    let mermaidDiagram =
    `sequenceDiagram
         box OpenAI Semantic Functions
         participant OpenAI
         ${semanticFunctions}
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
            result += '<br/>';
        }
        result += input[i];
    }
    return result;
}
