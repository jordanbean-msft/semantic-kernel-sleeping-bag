import * as React from "react";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableContainer from "@mui/material/TableContainer";
import TableHead from "@mui/material/TableHead";
import TableRow from "@mui/material/TableRow";
import Paper from "@mui/material/Paper";
import Box from "@mui/material/Box";
import Collapse from "@mui/material/Collapse";
import IconButton from "@mui/material/IconButton";
import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";
import KeyboardArrowUpIcon from "@mui/icons-material/KeyboardArrowUp";
import ResponseGraph from "../ResponseGraph/ResponseGraph";
import Container from "@mui/material/Container";
import { ResponseMessage } from "../../@types/ResponseMessage";

interface ThoughtProcessProps {
    response: ResponseMessage | undefined;
}

export default function ThoughtProcess({ response }: ThoughtProcessProps) {
    return (
        <div>
            <Container maxWidth={false}>
                <ResponseGraph response={response} />
            </Container>
            <TableContainer component={Paper}>
                <Table sx={{ minWidth: 650 }} size="small" aria-label="a dense table">
                    <TableHead>
                        <TableRow>
                            <TableCell />
                            <TableCell>Role</TableCell>
                            <TableCell>Function Name</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {response?.chatHistory?.map(
                            (row: { content: React.Key | null | undefined }) => (
                                <Row row={row} />
                            )
                        )}
                    </TableBody>
                </Table>
            </TableContainer>
        </div>
    );
}

function Row(props: { row: any }) {
    const { row } = props;
    const [open, setOpen] = React.useState(false);

    return (
        <React.Fragment>
            <TableRow sx={{ "& > *": { borderBottom: "unset" } }}>
                <TableCell>
                    <IconButton
                        aria-label="expand row"
                        size="small"
                        onClick={() => setOpen(!open)}
                    >
                        {open ? <KeyboardArrowUpIcon /> : <KeyboardArrowDownIcon />}
                    </IconButton>
                </TableCell>
                <TableCell scope="row">{row.role}</TableCell>
                <TableCell>{row.functionName}</TableCell>
            </TableRow>
            <TableRow>
                <TableCell style={{ paddingBottom: 0, paddingTop: 0 }} colSpan={6}>
                    <Collapse in={open} timeout="auto" unmountOnExit>
                        <Box sx={{ margin: 1 }}>
                            <Table size="small" aria-label="purchases">
                                <TableHead>
                                    <TableRow>
                                        <TableCell>Role</TableCell>
                                        <TableCell>Content</TableCell>
                                        <TableCell>FunctionName</TableCell>
                                        <TableCell>FunctionArguments</TableCell>
                                    </TableRow>
                                </TableHead>
                                <TableBody>
                                    <TableCell scope="row">{row.role}</TableCell>
                                    <TableCell>{row.content}</TableCell>
                                    <TableCell>{row.functionName}</TableCell>
                                    <TableCell>{row.functionArguments}</TableCell>
                                </TableBody>
                            </Table>
                        </Box>
                    </Collapse>
                </TableCell>
            </TableRow>
        </React.Fragment>
    );
}
