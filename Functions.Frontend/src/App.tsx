import React, { useState } from "react";
import "./App.css";
import axios from "axios";
import {
  Button,
  TextField,
  Card,
  Snackbar,
  CircularProgress,
  CardContent,
  Box,
  Link,
  Typography,
  Alert
} from "@mui/material"
import { Launch  } from "@mui/icons-material"

function App() {
  const [azureDevopsToken, setAzureDevopsToken] = useState("");
  const [azureDevOpsOrganization, setAzureDevOpsOrganization] = useState("");
  const [azureDevOpsAreaPath, setAzureDevOpsAreaPath] = useState("");
  const [gitHubToken, setGitHubToken] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  return (
    <div className="App">
      <header className="App-header">
        <Box width={1 / 2}>
          <Card>
            <CardContent>
              <Typography
                style={{ fontSize: 14 }}
                color="textSecondary"
                gutterBottom
              >
                Import the first 50 product backlog items from Azure DevOps into
                GitHub
              </Typography>
              <form
                onSubmit={e => {
                  e.preventDefault(); // prevent from onSubmit errors in tests
                  setLoading(true);
                  axios
                    .post("https://azdo2gh.azurewebsites.net/api/migrate", {
                      azureDevOps: {
                        token: azureDevopsToken,
                        organization: azureDevOpsOrganization,
                        areaPath: azureDevOpsAreaPath
                      },
                      github: {
                        token: gitHubToken
                      }
                    })
                    .then(r =>
                      r.data?.runtimeStatus === "Failed"
                        ? setError("function failed")
                        : setError("")
                    )
                    .catch(x => setError(x.toString()))
                    .finally(() => setLoading(false));
                }}
              >
                <TextField
                  label="organization"
                  id="azure-devops-organization"
                  value={azureDevOpsOrganization}
                  required={true}
                  variant="standard"
                  fullWidth
                  onChange={x =>
                    setAzureDevOpsOrganization(x.currentTarget.value)
                  }
                />
                <TextField
                  label="azure devops token"
                  id="azure-devops-token"
                  value={azureDevopsToken}
                  onChange={x => setAzureDevopsToken(x.currentTarget.value)}
                  required={true}
                  variant="standard"
                  fullWidth
                  helperText={
                    <span>
                      scope: Work Item (Read)&nbsp;
                      <Link
                        href={
                          "https://dev.azure.com/" +
                          (azureDevOpsOrganization !== ""
                            ? azureDevOpsOrganization + "/_usersSettings/tokens"
                            : "")
                        }
                        target="_blank"
                        rel="noopener noreferrer"
                        data-testid="azure-devops-token-url"
                      >
                        <Launch style={{ fontSize: 12 }} />
                      </Link>
                    </span>
                  }
                />
                <TextField
                  label="area path"
                  id="azure-devops-areapath"
                  value={azureDevOpsAreaPath}
                  required={true}
                  variant="standard"
                  fullWidth
                  onChange={x => setAzureDevOpsAreaPath(x.currentTarget.value)}
                  helperText="query product backlog items under area path"
                />
                <TextField
                  label="github token"
                  id="github-token"
                  value={gitHubToken}
                  required={true}
                  variant="standard"
                  fullWidth
                  onChange={x => setGitHubToken(x.currentTarget.value)}
                  helperText={
                    <span>
                      scope: public_repo&nbsp;
                      <Link
                        href="https://github.com/settings/tokens"
                        target="_blank"
                        rel="noopener noreferrer"
                      >
                        <Launch style={{ fontSize: 12 }} />
                      </Link>
                    </span>
                  }
                />
                <Button
                  disabled={
                    loading ||
                    [
                      azureDevopsToken,
                      azureDevOpsOrganization,
                      azureDevOpsAreaPath,
                      gitHubToken
                    ].includes("")
                  }
                  type="submit"
                >
                  Go!
                </Button>
              </form>
            </CardContent>
          </Card>
        </Box>
        <Snackbar
          open={error !== ""}
          autoHideDuration={6000}
          onClose={() => setError("")}
          data-testid="error"
        >
          <Alert severity="error">{error}</Alert>
        </Snackbar>
        <Snackbar
          open={loading}
          autoHideDuration={6000}
          onClose={() => setLoading(false)}
          data-testid="loading"
        >
          <CircularProgress />
        </Snackbar>
      </header>
    </div>
  );
}

export default App;
