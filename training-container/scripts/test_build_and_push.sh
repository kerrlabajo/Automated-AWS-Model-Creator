# Setup
test_setup() {
    MOCK_DIR="./mockbin"
    mkdir -p "$MOCK_DIR"
    export PATH="$MOCK_DIR:$PATH"

    # Mock docker command
    echo '#!/bin/bash' > "$MOCK_DIR/docker"
    echo 'echo "$@"' >> "$MOCK_DIR/docker"
    chmod +x "$MOCK_DIR/docker"

    # Mock aws command
    echo '#!/bin/bash' > "$MOCK_DIR/aws"
    echo 'echo "$@"' >> "$MOCK_DIR/aws"
    chmod +x "$MOCK_DIR/aws"
}

# Test Argument Handling
test_argument_handling() {
    ./build_and_push.sh > /dev/null 2>&1
    if [ $? -eq 0 ]; then
        echo "Test fail: Arguments Handling."
        exit 1
    else
        echo "Test pass: Arguments Handling."
    fi
}

# Test Docker Build Commands
test_docker_build_commands() {
    output=$(./build_and_push.sh my_account_id us-west-2 my_repo_name my_tag)
    expected_build_cmds="build -f ../docker/ubuntu-cuda/Dockerfile -t ubuntu-cuda ../docker/ubuntu-cuda build -f ../docker/yolov5/Dockerfile -t my_repo_name ../docker/yolov5"

    # echo "Actual output: $output"
    # echo "Expected to include: $expected_build_cmds"

    if [[ $output == *"$expected_build_cmds"* ]]; then
        echo "Test pass: Docker build commands are correct."
    else
        echo "Test fail: Docker build commands are not correct."
        exit 1
    fi
}

# Teardown
test_teardown() {
    rm -rf "$MOCK_DIR"
}

# Run
test_setup
test_argument_handling
test_docker_build_commands
test_teardown

echo "All tests passed."