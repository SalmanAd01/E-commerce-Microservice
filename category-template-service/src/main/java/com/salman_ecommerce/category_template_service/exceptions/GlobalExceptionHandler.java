package com.salman_ecommerce.category_template_service.exceptions;

import java.sql.SQLIntegrityConstraintViolationException;
import java.time.LocalDateTime;

import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.http.HttpStatus;
import org.springframework.http.HttpStatusCode;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.ControllerAdvice;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.servlet.NoHandlerFoundException;

@ControllerAdvice
public class GlobalExceptionHandler {

    @ExceptionHandler(ResourceNotFoundException.class)
    public ResponseEntity<ErrorResponse> handleResourceNotFound(ResourceNotFoundException ex) {
        HttpStatus status = HttpStatus.NOT_FOUND;
        return new ResponseEntity<>(
                new ErrorResponse(
                        LocalDateTime.now(),
                        status.value(),
                        status.getReasonPhrase(),
                        ex.getMessage()
                ),
                status
        );
    }

    @ExceptionHandler(BadRequestException.class)
    public ResponseEntity<ErrorResponse> handleBadRequest(BadRequestException ex) {
        HttpStatus status = HttpStatus.BAD_REQUEST;
        return new ResponseEntity<>(
                new ErrorResponse(
                        LocalDateTime.now(),
                        status.value(),
                        status.getReasonPhrase(),
                        ex.getMessage()
                ),
                status
        );
    }

        @ExceptionHandler(DataIntegrityViolationException.class)
        public ResponseEntity<ErrorResponse> handleDataIntegrityViolation(DataIntegrityViolationException ex) {
                HttpStatus status = HttpStatus.BAD_REQUEST;
                String message = "Database constraint violation";
                Throwable root = ex.getRootCause();
                if (root != null && root.getMessage() != null) {
                        message = root.getMessage();
                } else if (ex.getMessage() != null) {
                        message = ex.getMessage();
                }

                ErrorResponse errorResponse = new ErrorResponse(
                                LocalDateTime.now(),
                                status.value(),
                                status.getReasonPhrase(),
                                message
                );
                return new ResponseEntity<>(errorResponse, status);
        }

        @ExceptionHandler({SQLIntegrityConstraintViolationException.class, org.hibernate.exception.ConstraintViolationException.class})
        public ResponseEntity<ErrorResponse> handleSqlConstraintViolation(Exception ex) {
                HttpStatus status = HttpStatus.BAD_REQUEST;
                String message = ex.getMessage() != null ? ex.getMessage() : "Constraint violation";
                ErrorResponse errorResponse = new ErrorResponse(
                                LocalDateTime.now(),
                                status.value(),
                                status.getReasonPhrase(),
                                message
                );
                return new ResponseEntity<>(errorResponse, status);
        }

    @ExceptionHandler(Exception.class)
    public ResponseEntity<ErrorResponse> handleAllExceptions(Exception ex) {
        HttpStatusCode status = HttpStatus.INTERNAL_SERVER_ERROR;
        String errorMessage = (status instanceof HttpStatus httpStatus)
                ? httpStatus.getReasonPhrase()
                : "Unexpected Error";

        ErrorResponse errorResponse = new ErrorResponse(
                LocalDateTime.now(),
                status.value(),
                errorMessage,
                ex.getMessage()
        );

        return new ResponseEntity<>(errorResponse, status);
    }

    @ExceptionHandler(NoHandlerFoundException.class)
    public ResponseEntity<ErrorResponse> handleNoHandlerFound(NoHandlerFoundException ex) {
        HttpStatus status = HttpStatus.NOT_FOUND;
        ErrorResponse errorResponse = new ErrorResponse(
                LocalDateTime.now(),
                status.value(),
                status.getReasonPhrase(),
                "Endpoint not found: " + ex.getRequestURL()
        );
        return new ResponseEntity<>(errorResponse, status);
    }
}
