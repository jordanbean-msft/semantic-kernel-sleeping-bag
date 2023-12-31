import { useState } from "react";
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

export interface OpenAIMessage {
  action: string;
  final_answer: string;
  thought: string;
  original_response: string;
  observation: string;
}

export interface ResponseMessage {
  openAIMessages: Array<OpenAIMessage>;
}

export default function Response({ response }: { response: ResponseMessage }) {
  return (
    <TableContainer component={Paper}>
      <Table sx={{ minWidth: 650 }} size="small" aria-label="a dense table">
        <TableHead>
          <TableRow>
            <TableCell />
            <TableCell>Action</TableCell>
            <TableCell>Final Answer</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {response?.openAIMessages?.map(
            (row: { thought: React.Key | null | undefined }) => (
              <Row row={row} />
            )
          )}
        </TableBody>
      </Table>
    </TableContainer>
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
        <TableCell scope="row">{row.action}</TableCell>
        <TableCell>{row.final_answer}</TableCell>
      </TableRow>
      <TableRow>
        <TableCell style={{ paddingBottom: 0, paddingTop: 0 }} colSpan={6}>
          <Collapse in={open} timeout="auto" unmountOnExit>
            <Box sx={{ margin: 1 }}>
              <Table size="small" aria-label="purchases">
                <TableHead>
                  <TableRow>
                    <TableCell>Thought</TableCell>
                    <TableCell>Observation</TableCell>
                    <TableCell>Original Response</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  <TableCell scope="row">{row.thought}</TableCell>
                  <TableCell>{row.observation}</TableCell>
                  <TableCell>{row.original_response}</TableCell>
                </TableBody>
              </Table>
            </Box>
          </Collapse>
        </TableCell>
      </TableRow>
    </React.Fragment>
  );
}
