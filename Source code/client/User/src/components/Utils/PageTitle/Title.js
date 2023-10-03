import React, { useCallback } from "react";
import { useNavigate } from "react-router-dom";
import styles from "./Title.module.css";
import { Tooltip } from "@mui/material";
import KeyboardBackspaceIcon from "@mui/icons-material/KeyboardBackspace";

const Title = ({ pageTitle }) => {
  let navigate = useNavigate();
  const goBack = useCallback(() => navigate(-1), [navigate]);

  return (
    <div className={styles.container} data-testid='titlecomponenet'>
      <div>
        <Tooltip title="Go back">
          <KeyboardBackspaceIcon className={styles.backicon} data-testid='gobackbutton' onClick={goBack} />
        </Tooltip>
      </div>
      <div className={styles.title}>{pageTitle}</div>
    </div>
  );
};

export default Title;
